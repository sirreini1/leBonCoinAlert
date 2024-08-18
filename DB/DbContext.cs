using Microsoft.EntityFrameworkCore;

namespace LeBonCoinAlert.DB;

public class AppDbContext : DbContext
{
    public DbSet<FlatAdEntity> FlatAds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=LeBonCoinAlert.db");
    }
}