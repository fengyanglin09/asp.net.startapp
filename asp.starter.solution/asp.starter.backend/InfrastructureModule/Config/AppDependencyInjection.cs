using asp.starter.backend.InfrastructureModule.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace asp.starter.backend.InfrastructureModule.Config;

public static class AppDependencyInjection
{

    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration config)
    {
        // Register infrastructure services here (e.g., database contexts, repositories, external API clients, etc.)
        // Example: register DB, repositories, clients, etc.
        // services.AddDbContext<AppDbContext>(...);
        // services.AddScoped<IUserRepository, UserRepository>();


        services.AddOptions<DatabaseOptions>()
            .Bind(config.GetSection("Database")) // or DatabaseSettings if that’s your chosen name
            .PostConfigure(options =>
            {
                options.Username = config["DB_USER"] ?? "";
                options.Password = config["DB_PASS"] ?? "";
                options.Host = config["DB_HOST"] ?? "127.0.0.1";
            })
            .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "Database:Host is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Name), "Database:Name is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Username), "Database:Username is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Password), "Database:Password is required")
            .ValidateOnStart();
        
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var db = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;

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

            options.UseNpgsql(csb.ConnectionString);
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