using asp.starter.backend.ApplicationModule.Config;
using asp.starter.backend.AuthModule.Config;
using asp.starter.backend.InfrastructureModule.Config;

namespace asp.starter.backend;

public static class AppConfigModule
{
    //DI registrations
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        
        // feature / layer modules
        services.AddApplicationModule();
        services.AddInfrastructureModule(configuration);
        services.AddAuthModule(configuration);
        
        // cross-cutting concerns - logging, authentication, authorization, caching, etc.
        
        
        services.AddEndpointsApiExplorer();
        
        return services;
    }
    
    
    // HTTP request pipeline configuration and middleware
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            app.MapOpenApi();
        }
        
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

}