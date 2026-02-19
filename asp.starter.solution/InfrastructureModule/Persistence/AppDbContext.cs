using Microsoft.EntityFrameworkCore;

namespace asp.starter.backend.InfrastructureModule.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    // Add DbSets when you have entities
    // public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent mappings go here if needed
        // modelBuilder.Entity<User>().ToTable("user");
    }
}