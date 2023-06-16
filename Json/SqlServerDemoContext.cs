﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Json;

public class SqlServerDemoContext : DbContext
{
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(Console.WriteLine, (_, level) => level == LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseSqlServer("server=localhost;database=EFDemo;Trusted_Connection=True;Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().Property(e => e.LastName).HasMaxLength(50);
        modelBuilder.Entity<Employee>().Property(e => e.FirstName).HasMaxLength(50);
        modelBuilder.Entity<Employee>().Property(e => e.Department).HasMaxLength(50);
        //modelBuilder.Entity<Employee>().Property(e => e.Contacts).StoreAsJson();
        modelBuilder.Entity<Employee>().Property(e => e.AddressDetails).StoreAsJson();

        modelBuilder.Entity<Employee>().OwnsOne(e => e.BillingAddress).ToJson();
        modelBuilder.Entity<Employee>().OwnsOne(e => e.PrimaryContact).ToJson().OwnsOne(builder => builder.Rules);
        modelBuilder.Entity<Employee>().OwnsMany(e => e.Contacts).ToJson().OwnsOne(builder => builder.Rules);

        modelBuilder.Entity<Employee>().Property(e => e.State)
            .HasComputedColumnSql("JSON_VALUE([BillingAddress], '$.State')");
        
        modelBuilder.Entity<Employee>().HasIndex(e => e.State);
    }
}