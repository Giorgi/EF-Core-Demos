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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>().Property(p => p.SearchDocument)
            .HasComputedColumnSql($@"setweight(to_tsvector('english', ""{nameof(Post.Title)}""), 'A') || 
                                     setweight(to_tsvector('english', ""{nameof(Post.Body)}""), 'B')  || 
                                     setweight(to_tsvector('simple', ""{nameof(Post.Author)}""), 'C') || 
                                     setweight(to_tsvector('simple', ""{nameof(Post.Tags)}""), 'B') ", true);

        modelBuilder.Entity<Post>().HasIndex(p => p.SearchDocument).HasMethod("GIN");
    }
}