using Microsoft.EntityFrameworkCore;

namespace asp.starter.backend.InfrastructureModule.Config;

public class AppDbContext(DbContextOptions options) : DbContext(options);