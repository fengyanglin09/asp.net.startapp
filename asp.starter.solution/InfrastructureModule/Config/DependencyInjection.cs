namespace asp.starter.backend.InfrastructureModule.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register infrastructure services here (e.g., database contexts, repositories, external API clients, etc.)
        // Example: register DB, repositories, clients, etc.
        // services.AddDbContext<AppDbContext>(...);
        // services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
    
}