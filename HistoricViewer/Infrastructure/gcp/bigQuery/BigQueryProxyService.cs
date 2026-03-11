

using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Options;

namespace HistoricViewer.Infrastructure.gcp.bigQuery;

// ── Interface ─────────────────────────────────────────────────────────────────

public interface IBigQueryProxyService
{
    Task<BigQueryResult> ExecuteQueryAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        BigQueryQueryOptions? options = null);
}

// ── Implementation ────────────────────────────────────────────────────────────

public class BigQueryProxyService(
    IOptions<BigQueryServiceOptions> config,
    ILogger<BigQueryProxyService> logger)
    : IBigQueryProxyService
{
    private readonly BigQueryServiceOptions _config = config.Value;

    // Cached impersonated client — refreshed before the 1-hour token expires
    private BigQueryClient? _client;
    private DateTime _clientExpiry = DateTime.MinValue;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<BigQueryResult> ExecuteQueryAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        BigQueryQueryOptions? options = null)
    {
        try
        {
            var client = await GetOrCreateClientAsync();

            //set up query options and results options based on provided options or defaults
            var queryOptions = new QueryOptions
            {
                UseLegacySql = options?.UseLegacySql ?? false
            };
            
            //set up results options with timeout and page size if provided; otherwise use defaults
            var resultsOptions = new GetQueryResultsOptions
            {
                Timeout = options?.Timeout ?? TimeSpan.FromSeconds(60) // 1 minute default timeout for query execution
            };
            if (options?.PageSize is int ps)
                resultsOptions.PageSize = ps;

            logger.LogInformation(
                "Executing BigQuery on project {Project}: {Sql}",
                _config.TargetProjectId, sql);

            // parameters are passed as a dictionary of name-value pairs; convert to BigQueryParameter list if not null
            // they are part of the query execution request and will be used to safely parameterize the query, preventing SQL injection and ensuring proper typing

            var bigQueryParameters = parameters?.Select(kvp => CreateParameter(kvp.Key, kvp.Value)).ToList();

            BigQueryResults results = await client.ExecuteQueryAsync(
                sql,
                bigQueryParameters,        // IEnumerable<BigQueryParameter>? — pass null for parameterless
                queryOptions,
                resultsOptions);

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
                Success = true,
                Rows = rows,
                
                // The schema might look like:
                // Field 1: Name = "id", Type = "INT64", Mode = "REQUIRED"
                // Field 2: Name = "name", Type = "STRING", Mode = "NULLABLE"
                // Field 3: Name = "age", Type = "INT64", Mode = "NULLABLE"
                
                Schema = results.Schema?.Fields?.Select(f => new BigQueryColumnInfo
                {
                    Name = f.Name,
                    Type = f.Type,
                    Mode = f.Mode ?? "NULLABLE"
                }).ToList(),
                TotalRows = rows.Count,
                //  BytesProcessed lives on the job; BigQueryResults exposes it via
                //    the underlying job reference — safest is to just use rows.Count
                //    or pull from the job separately. Set 0 to avoid the missing member.
                BytesProcessed = 0
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "BigQuery query failed");
            return new BigQueryResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    // ── Client creation ───────────────────────────────────────────────────────

    private async Task<BigQueryClient> GetOrCreateClientAsync()
    {
        //first check without locking for performance; if the client is valid, return immediately; if not, acquire lock to create/refresh the client; this avoids unnecessary locking on every query while still ensuring thread safety when refreshing the client
        if (_client is not null && DateTime.UtcNow < _clientExpiry)
            return _client;
        //allow only one thread at a time to create/refresh the client to avoid redundant work and potential rate limits; other threads will wait for the lock and then use the refreshed client
        await _lock.WaitAsync();
        try
        {
        
            //Multiple threads could reach the first check at the same time, find the client expired or null, and then all wait for the lock.
            // When the first thread acquires the lock, it will refresh the client.
            // The next waiting thread, after acquiring the lock, should check again if the client is now valid (because the first thread may have already refreshed it).
            // This prevents unnecessary creation of multiple clients and ensures only one refresh happens.
            
            if (_client is not null && DateTime.UtcNow < _clientExpiry)
                return _client;

            //service account token is created with a specific lifetime using the ImpersonatedCredential.Initializer for 1 hour
            _client = await CreateImpersonatedClientAsync();
            
            _clientExpiry = DateTime.UtcNow.AddMinutes(55); // Refresh before 1-hr token expires
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

        // Step 1 — Cloud Run SA credential via ADC (no key file needed on Cloud Run)
        GoogleCredential sourceCredential = await GoogleCredential.GetApplicationDefaultAsync();

        // Step 2 — Impersonate the BigQuery SA
        //  Use .Impersonate() on GoogleCredential — ImpersonatedCredential.Create() is internal
        GoogleCredential impersonatedCredential = sourceCredential.Impersonate(
            new ImpersonatedCredential.Initializer(_config.BigQueryServiceAccountEmail)
            {
                //OAuth 2.0 scopes used when authenticating with APIs
                //this is telling which part of gcp services we (the app) wanted to access
                Scopes = new[]
                {
                    //this scope grants full access to BigQuery - Note it will not override IAM roles
                    "https://www.googleapis.com/auth/bigquery",
                    "https://www.googleapis.com/auth/bigquery.readonly"
                },
                //this is to set the token to be expired in 1 hour
                Lifetime = TimeSpan.FromHours(1)
            });

        // Step 3 — BigQueryClient.CreateAsync accepts GoogleCredential directly
        // Impersonate() already returns GoogleCredential — no .ToGoogleCredential() needed
        return await BigQueryClient.CreateAsync(_config.TargetProjectId, impersonatedCredential);
    }
    
    // ── Parameter helpers ────────────────────────────────────────────────────

    private static BigQueryParameter CreateParameter(string name, object value) =>
        value switch
        {
            int i       => new BigQueryParameter(name, BigQueryDbType.Int64, (long)i),
            long l      => new BigQueryParameter(name, BigQueryDbType.Int64, l),
            double d    => new BigQueryParameter(name, BigQueryDbType.Float64, d),
            float f     => new BigQueryParameter(name, BigQueryDbType.Float64, (double)f),
            bool b      => new BigQueryParameter(name, BigQueryDbType.Bool, b),
            DateTime dt => new BigQueryParameter(name, BigQueryDbType.Timestamp, DateTime.SpecifyKind(dt, DateTimeKind.Utc)),
            DateOnly d  => new BigQueryParameter(name, BigQueryDbType.Date, d.ToDateTime(TimeOnly.MinValue)),
            _           => new BigQueryParameter(name, BigQueryDbType.String, value?.ToString())
        };
    
}