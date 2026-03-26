namespace HistoricViewer.Infrastructure.gcp.bigQuery;

public static class BigQueryProxyServiceConfigurations
{
    public static IServiceCollection AddBigQueryProxyService(this IServiceCollection services, IConfiguration configuration)
    {

        // making sure the options can be injected        
        services.AddOptions<BigQueryServiceOptions>()
            .Bind(configuration.GetSection(BigQueryServiceOptions.SectionName))
            .PostConfigure(options =>
            {
                
            })
            .Validate(o => !string.IsNullOrWhiteSpace(o.BigQueryServiceAccountEmail), $"{BigQueryServiceOptions.SectionName}:BigQueryServiceAccountEmail is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.TargetProjectId), $"{BigQueryServiceOptions.SectionName}:TargetProjectId is required")
            .ValidateOnStart();
        

        // note: can not be scoped, need to be Singleton — preserves circuit breaker state and cached BigQuery client across requests
        services.AddSingleton<IBigQueryProxyService, BigQueryProxyService>();

        return services;
    }
    
}