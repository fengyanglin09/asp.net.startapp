namespace HistoricViewer.Application.Configs;

public static class AppConfigurations
{

    public static IServiceCollection AddApplicationConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddControllers();
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        return services;
        
    }

}