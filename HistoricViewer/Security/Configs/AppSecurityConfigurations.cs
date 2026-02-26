namespace HistoricViewer.Security.Configs;

public static class AppSecurityConfigurations
{

    public static IServiceCollection AddSecurityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication();
        services.AddAuthorization();
        return services;
    }

}