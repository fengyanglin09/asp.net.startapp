namespace HistoricViewer;

public static class AppConfigModule
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
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

        return app;
    }
    
}