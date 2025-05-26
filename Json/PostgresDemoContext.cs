using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Json;

public class PostgresDemoContext : DbContext
{
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();

        optionsBuilder
            .LogTo(Console.WriteLine, (_, level) => level == LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseNpgsql("Host=localhost;Database=EFCoreDemo;Username=postgres;Password=12345678");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().Property(e => e.LastName).HasMaxLength(50);
        modelBuilder.Entity<Employee>().Property(e => e.FirstName).HasMaxLength(50);
        modelBuilder.Entity<Employee>().Property(e => e.Department).HasMaxLength(50);

        modelBuilder.Entity<Employee>().Property(e => e.BillingAddress).HasColumnType("jsonb");
        modelBuilder.Entity<Employee>().Property(e => e.PrimaryContact).HasColumnType("jsonb");
        modelBuilder.Entity<Employee>().Property(e => e.Contacts).HasColumnType("jsonb");
        
    }
}