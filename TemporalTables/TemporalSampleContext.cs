using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TemporalTables;

public class TemporalSampleContext : DbContext
{
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Address> Addresses=> Set<Address>();


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(Console.WriteLine, (_, level) => level == LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseSqlServer("server=(localdb)\\ProjectModels;database=EFDemoTemporal;Trusted_Connection=True;Encrypt=false");

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<string>().HaveMaxLength(50);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().ToTable(b => b.IsTemporal());
        modelBuilder.Entity<Contact>().ToTable(b => b.IsTemporal());
        modelBuilder.Entity<Address>().ToTable(b => b.IsTemporal());
    }
}
