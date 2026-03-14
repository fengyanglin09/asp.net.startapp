using HistoricViewer.Application.Configs;
using HistoricViewer.Infrastructure.Configs;
using HistoricViewer.Infrastructure.gcp.bigQuery;
using HistoricViewer.Security.Configs;
using Microsoft.EntityFrameworkCore;

namespace HistoricViewer;

public static class AppConfigModule
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddApplicationConfigurations(configuration);
        services.AddApplicationDependencyInjection(configuration);
        services.AddSecurityModule(configuration);
        services.AddDatabaseModule(configuration);
        services.AddBigQueryProxyService(configuration);
        services.AddSwaggerConfigurations(configuration);
        
        return services;
    }
    
    
}