using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace HistoricViewer.Infrastructure.Configs;

public static class AppDatabaseConfigurations
{
    public static IServiceCollection AddDatabaseModule(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection("Database"))
            .PostConfigure(options =>
            {
                //populate from env vars if not set in config
                options.Username = configuration["DB_USER"] ?? "";
                options.Password = configuration["DB_PASS"] ?? "";
                options.Host = configuration["DB_HOST"] ?? "127.0.0.1";
            })
            .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "Database:Host is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Name), "Database:Name is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Username), "Database:Username is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Password), "Database:Password is required")
            .ValidateOnStart();

        services.AddDbContext<AppDbContext>((serviceProvider, optionsBuilder) =>
        {
            var db = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

            var csb = new NpgsqlConnectionStringBuilder
            {
                Host = db.Host,
                Port = db.Port,
                Database = db.Name,
                Username = db.Username,
                Password = db.Password,
                Pooling = db.Pooling,
                MinPoolSize = db.MinPoolSize,
                MaxPoolSize = db.MaxPoolSize,          // <-- correct property name
                CommandTimeout = db.CommandTimeoutSeconds
            };

            optionsBuilder.UseNpgsql(csb.ConnectionString);
        });
        
        return services;
    }
}


public sealed class DatabaseOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 5432;
    public string Name { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public bool Pooling { get; set; } = true;
    public int MinPoolSize { get; set; } = 5;
    public int MaxPoolSize { get; set; } = 20;
    public int CommandTimeoutSeconds { get; set; } = 30;
}