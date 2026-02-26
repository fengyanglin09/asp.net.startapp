namespace HistoricViewer.Application.Configs;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<GreetingService>();
        return services;
    }
}

// this is an example of a service injected into the application.

public sealed class GreetingService
{
    public string Greet(string name) => $"Hello {name}";
}