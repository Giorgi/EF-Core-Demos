using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Json;

public class PostgresDemoContext : DbContext
{
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(Console.WriteLine, (_, level) => level == LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseNpgsql("Host=localhost;Database=EFCoreDemo;Username=postgres;Password=qwerty");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().Property(e => e.LastName).HasMaxLength(50);
        modelBuilder.Entity<Employee>().Property(e => e.FirstName).HasMaxLength(50);
        modelBuilder.Entity<Employee>().Property(e => e.Department).HasMaxLength(50);
        
        //modelBuilder.Entity<Employee>().Property(e => e.AddressDetails).HasColumnType("jsonb");

        modelBuilder.Entity<Employee>().Property(e => e.BillingAddress).HasColumnType("jsonb");
        modelBuilder.Entity<Employee>().Property(e => e.PrimaryContact).HasColumnType("jsonb");
        modelBuilder.Entity<Employee>().Property(e => e.Contacts).HasColumnType("jsonb");
        
    }
}