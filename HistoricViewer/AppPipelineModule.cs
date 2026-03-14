using System.Collections.Immutable;
using HistoricViewer.Application.Configs;
using HistoricViewer.Infrastructure.Configs;
using HistoricViewer.Infrastructure.gcp.bigQuery;
using HistoricViewer.Security.Configs;
using Microsoft.EntityFrameworkCore;

namespace HistoricViewer;

public static class AppPipelineModule
{
    
    
    private static readonly ImmutableList<string> PublicPaths =
        ImmutableList.Create("/swagger", "/public", "/favicon.ico");
    
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        
        
        app.UseHttpsRedirection();

        app.UseRouting();
        
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
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

        }).AllowAnonymous();
        
        // Public health check endpoint, open to everyone
        app.MapGet("/public/health", () => "OK").AllowAnonymous();
       
        // Enable Scalar Swagger UI

        PublicPaths.ToList().ForEach(path =>
        {
            app.MapGroup(path).AllowAnonymous();
        });
        
        return app;
    }
    
}