namespace asp.starter.backend.AuthModule.Config;

public static class DependencyInjection
{

    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication();
        services.AddAuthorization();
        return services;
    }

}