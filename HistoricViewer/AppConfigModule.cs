using HistoricViewer.Application.Configs;
using HistoricViewer.Infrastructure.Configs;
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
        return services;
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // if (app.Environment.IsDevelopment())
        // {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        // }
        //
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        // Log the current profile (environment name)
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("AppConfigModule");
        logger.LogInformation($"Current profile: {app.Environment.EnvironmentName}");
        
        // check if the database is up and running, if not, return a friendly error message

        app.MapGet("/db-ping", async (AppDbContext db, IWebHostEnvironment env) =>
        {
            try
            {
                await db.Database.OpenConnectionAsync();
                await db.Database.CloseConnectionAsync();
             
                return Results.Ok(new
                {
                    ok = true,
                    timestamp = DateTime.UtcNow,
                    profile = env.EnvironmentName,
                    message = "Database connection successful"
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: ex.GetType().Name,
                    detail: ex.Message
                );
            }

        });
        

        return app;
    }
    
}