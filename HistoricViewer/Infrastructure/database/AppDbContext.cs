using Microsoft.EntityFrameworkCore;

namespace HistoricViewer.Infrastructure.Configs;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

}