using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FTS;

public class FullTextSearchContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(Console.WriteLine, (_, level) => level == LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseNpgsql("Host=localhost;Database=EFCoreDemo;Username=postgres;Password=qwerty");

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
        
    //}
}