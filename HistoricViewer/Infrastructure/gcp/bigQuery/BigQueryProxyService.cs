using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using System.Net;
using Polly.Timeout;

namespace HistoricViewer.Infrastructure.gcp.bigQuery;

public interface IBigQueryProxyService
{
    Task<BigQueryResult> ExecuteQueryAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        BigQueryQueryOptions? options = null);
}

public class BigQueryProxyService(
    IOptions<BigQueryServiceOptions> config,
    ILogger<BigQueryProxyService> logger)
    : IBigQueryProxyService
{
    private readonly BigQueryServiceOptions _config = config.Value;

    private BigQueryClient? _client;
    private DateTime _clientExpiry = DateTime.MinValue;
    private readonly SemaphoreSlim _lock = new(1, 1);

    // ── Resilience pipeline ───────────────────────────────────────────────────

    // Not retried includes
    //
    // permission denied
    //
    // invalid SQL
    //
    // dataset not found
    //
    // bad config
    //
    // null reference
    //
    // 403 if not explicitly handled
    // Those go straight to outer catch.
    private readonly ResiliencePipeline _pipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay            = TimeSpan.FromSeconds(2),
            BackoffType      = DelayBackoffType.Exponential, // 2s → 4s → 8s
            UseJitter        = true,
            ShouldHandle     = new PredicateBuilder()
                .Handle<HttpRequestException>()
                .Handle<TaskCanceledException>()
                .Handle<Google.GoogleApiException>(ex =>
                    // Retry on transient Google API errors only
                    ex.HttpStatusCode == HttpStatusCode.ServiceUnavailable ||  // 503
                    ex.HttpStatusCode == HttpStatusCode.TooManyRequests    ||  // 429
                    ex.HttpStatusCode == HttpStatusCode.InternalServerError),  // 500
            OnRetry = args =>
            {
                // args.Context carries our logger key set at call site
                if (args.Context.Properties.TryGetValue(
                        new ResiliencePropertyKey<ILogger>("logger"), out var log))
                {
                    log.LogWarning(
                        "BigQuery retry {Attempt}/{Max} after {Delay}ms. Reason: {Reason}",
                        args.AttemptNumber + 1, 3,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Exception?.Message ?? "unknown");
                }
                return ValueTask.CompletedTask;
            }
        })
        .AddCircuitBreaker(new CircuitBreakerStrategyOptions
        {
            // Open circuit after 50% failure rate over 5+ requests in 30s window
            SamplingDuration  = TimeSpan.FromSeconds(30),
            FailureRatio      = 0.5,
            MinimumThroughput = 5,
            BreakDuration     = TimeSpan.FromSeconds(15),
            OnOpened = args =>
            {
                // Circuit is now open — fast-fail all calls for BreakDuration
                return ValueTask.CompletedTask;
            }
        })
        .AddTimeout(TimeSpan.FromSeconds(90)) // outer timeout across all retries -- this is total allowed time before throwing out the time out exception
        .Build();

    // ── Query execution ───────────────────────────────────────────────────────

    public async Task<BigQueryResult> ExecuteQueryAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        BigQueryQueryOptions? options = null)
    {
        try
        {
            // Pass logger into the resilience context so OnRetry can log
            var context = ResilienceContextPool.Shared.Get();
            context.Properties.Set(new ResiliencePropertyKey<ILogger>("logger"), logger);

            try
            {
                return await _pipeline.ExecuteAsync(async ct =>
                {
                    // Client creation is also inside the retry —
                    // if the token expired mid-flight, it will refresh and retry
                    // it recovers following: 
                    // 1. expired impersonated credential
                    // 2. stale client
                    // 3. auth refresh timing issue
                    var client = await GetOrCreateClientAsync();

                    var queryOptions = new QueryOptions
                    {
                        UseLegacySql = options?.UseLegacySql ?? false
                    };

                    var resultsOptions = new GetQueryResultsOptions
                    {
                        Timeout = options?.Timeout ?? TimeSpan.FromSeconds(60)
                    };
                    if (options?.PageSize is int ps)
                        resultsOptions.PageSize = ps;

                    logger.LogInformation(
                        "Executing BigQuery on project {Project}: {Sql}",
                        _config.TargetProjectId, sql);

                    var bigQueryParameters = parameters?
                        .Select(kvp => CreateParameter(kvp.Key, kvp.Value))
                        .ToList();

                    BigQueryResults results = await client.ExecuteQueryAsync(
                        sql,
                        bigQueryParameters,
                        queryOptions,
                        resultsOptions
                        );  

                    var rows = new List<Dictionary<string, object?>>();
                    foreach (var row in results)
                    {
                        var dict = new Dictionary<string, object?>();
                        foreach (var field in results.Schema.Fields)
                            dict[field.Name] = row[field.Name];
                        rows.Add(dict);
                    }

                    return new BigQueryResult
                    {
                        Success       = true,
                        Rows          = rows,
                        Schema        = results.Schema?.Fields?.Select(f => new BigQueryColumnInfo
                        {
                            Name = f.Name,
                            Type = f.Type,
                            Mode = f.Mode ?? "NULLABLE"
                        }).ToList(),
                        TotalRows     = rows.Count,
                        BytesProcessed = 0
                    };

                }, context);
            }
            finally
            {
                ResilienceContextPool.Shared.Return(context);
            }
        }
        catch (BrokenCircuitException ex)
        {
            // Circuit is open — BigQuery is consistently failing, stop trying
            logger.LogError(ex, "BigQuery circuit breaker is open — service unavailable");
            return new BigQueryResult
            {
                Success      = false,
                ErrorMessage = "BigQuery is temporarily unavailable. Please try again later."
            };
        }
        catch (TimeoutRejectedException ex)
        {
            // All retries exhausted and total timeout exceeded
            logger.LogError(ex, "BigQuery query timed out after retries");
            return new BigQueryResult
            {
                Success      = false,
                ErrorMessage = "BigQuery query timed out."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "BigQuery query failed after retries");
            return new BigQueryResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    // ── Client creation ───────────────────────────────────────────────────────

    private async Task<BigQueryClient> GetOrCreateClientAsync()
    {
        if (_client is not null && DateTime.UtcNow < _clientExpiry)
            return _client;

        await _lock.WaitAsync();
        try
        {
            if (_client is not null && DateTime.UtcNow < _clientExpiry)
                return _client;

            // ✅ Force refresh if we're inside a retry after an auth failure
            _client = null;
            _client  = await CreateImpersonatedClientAsync();
            _clientExpiry = DateTime.UtcNow.AddMinutes(55);
            return _client;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<BigQueryClient> CreateImpersonatedClientAsync()
    {
        logger.LogInformation(
            "Building impersonated BigQuery client. Target SA: {SA}, Project: {Project}",
            _config.BigQueryServiceAccountEmail, _config.TargetProjectId);

        GoogleCredential sourceCredential = await GoogleCredential.GetApplicationDefaultAsync();

        GoogleCredential impersonatedCredential = sourceCredential.Impersonate(
            new ImpersonatedCredential.Initializer(_config.BigQueryServiceAccountEmail)
            {
                Scopes = new[]
                {
                    "https://www.googleapis.com/auth/bigquery",
                    "https://www.googleapis.com/auth/bigquery.readonly"
                },
                Lifetime = TimeSpan.FromHours(1)
            });

        return await BigQueryClient.CreateAsync(_config.TargetProjectId, impersonatedCredential);
    }

    // ── Parameter helpers ─────────────────────────────────────────────────────

    private static BigQueryParameter CreateParameter(string name, object value) =>
        value switch
        {
            int i      => new BigQueryParameter(name, BigQueryDbType.Int64, (long)i),
            long l     => new BigQueryParameter(name, BigQueryDbType.Int64, l),
            double d   => new BigQueryParameter(name, BigQueryDbType.Float64, d),
            float f    => new BigQueryParameter(name, BigQueryDbType.Float64, (double)f),
            bool b     => new BigQueryParameter(name, BigQueryDbType.Bool, b),
            DateTime dt => new BigQueryParameter(name, BigQueryDbType.Timestamp, DateTime.SpecifyKind(dt, DateTimeKind.Utc)),
            DateOnly d => new BigQueryParameter(name, BigQueryDbType.Date, d.ToDateTime(TimeOnly.MinValue)),
            _          => new BigQueryParameter(name, BigQueryDbType.String, value?.ToString())
        };
}