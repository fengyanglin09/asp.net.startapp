namespace asp.starter.backend.ApplicationModule.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        services.AddScoped<GreetingService>();
        return services;
    }
}


// this is an example of a service that could be registered in the application module. In a real application, you would have more complex services that handle business logic, data access, etc.
public sealed class GreetingService
{
    public string Greet(string name) => $"Hello {name}";
}