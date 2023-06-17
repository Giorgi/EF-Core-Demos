using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HierarchicalData;

public class HierarchicalDataContext : DbContext
{
    public DbSet<OrganizationPosition> Positions => Set<OrganizationPosition>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(Console.WriteLine, (_, level) => level == LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseSqlServer("server=localhost;database=EFDemo;Trusted_Connection=True;Encrypt=false", b => b.UseHierarchyId());


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrganizationPosition>().Property(p => p.Name).HasMaxLength(70);

        var data = new List<OrganizationPosition>
        {
            new OrganizationPosition
            {
                Name = "CEO",
                Path = HierarchyId.Parse("/")
            },
            new OrganizationPosition
            {
                Name = "Chief Commercial Officer",
                Path = HierarchyId.Parse("/1/")
            },
            new OrganizationPosition
            {
                Name = "Global Retail",
                Path = HierarchyId.Parse("/1/1/")
            },
            new OrganizationPosition
            {
                Name = "Enterprise Sales",
                Path = HierarchyId.Parse("/1/2/")
            },
            new OrganizationPosition
            {
                Name = "CTO",
                Path = HierarchyId.Parse("/2/")
            },
            new OrganizationPosition
            {
                Name = "Head of Software Development",
                Path = HierarchyId.Parse("/2/1/")
            },
            new OrganizationPosition
            {
                Name = ".Net Team Lead",
                Path = HierarchyId.Parse("/2/1/1/")
            },
            new OrganizationPosition
            {
                Name = ".Net Senior Engineer",
                Path = HierarchyId.Parse("/2/1/1/1/")
            },
            new OrganizationPosition
            {
                Name = ".Net Junior Engineer",
                Path = HierarchyId.Parse("/2/1/1/2/")
            },
            new OrganizationPosition
            {
                Name = "Frontend Team Lead",
                Path = HierarchyId.Parse("/2/1/2/")
            },
            new OrganizationPosition
            {
                Name = "Head of AI, ChatGPT, BlockChain and other buzzwords",
                Path = HierarchyId.Parse("/2/2/")
            },
            new OrganizationPosition
            {
                Name = "Chief AI Bot",
                Path = HierarchyId.Parse("/2/2/1/")
            },
            new OrganizationPosition
            {
                Name = "Chief Prompt Engineer",
                Path = HierarchyId.Parse("/2/2/2/")
            },
            new OrganizationPosition
            {
                Name = "Chief Web3 Developer",
                Path = HierarchyId.Parse("/2/2/3/")
            },
            new OrganizationPosition
            {
                Name = "Chief Marketing Officer",
                Path = HierarchyId.Parse("/3/")
            },
            new OrganizationPosition
            {
                Name = "Marketing Communications Head",
                Path = HierarchyId.Parse("/3/1/")
            },
            new OrganizationPosition
            {
                Name = "Marketing Communications Sr. Officer",
                Path = HierarchyId.Parse("/3/1/1/")
            },
            new OrganizationPosition
            {
                Name = "Marketing Social Communications Sr. Officer",
                Path = HierarchyId.Parse("/3/1/2/")
            },
            new OrganizationPosition
            {
                Name = "Marketing Communications Jr. Officer",
                Path = HierarchyId.Parse("/3/1/3/")
            },
            new OrganizationPosition
            {
                Name = "Marketing Communications Intern",
                Path = HierarchyId.Parse("/3/1/1/1/")
            },
            new OrganizationPosition
            {
                Name = "Marketing Advertising Head",
                Path = HierarchyId.Parse("/3/2/")
            },
            new OrganizationPosition
            {
                Name = "Advertising Agency Director",
                Path = HierarchyId.Parse("/3/2/1/")
            },
            new OrganizationPosition
            {
                Name = "Advertising Agency Specialist",
                Path = HierarchyId.Parse("/3/2/1/1/")
            },
            new OrganizationPosition
            {
                Name = "Marketing Brand Head",
                Path = HierarchyId.Parse("/3/3/")
            },
            new OrganizationPosition
            {
                Name = "Marketing Brand Manager",
                Path = HierarchyId.Parse("/3/3/1/")
            },
            new OrganizationPosition
            {
                Name = "CFO",
                Path = HierarchyId.Parse("/4/")
            },
            new OrganizationPosition
            {
                Name = "Corporate Vice President Finances",
                Path = HierarchyId.Parse("/4/1/")
            },
            new OrganizationPosition
            {
                Name = "Chief Accountant",
                Path = HierarchyId.Parse("/4/1/1/")
            },
            new OrganizationPosition
            {
                Name = "Accounting Manager",
                Path = HierarchyId.Parse("/4/1/1/1/")
            },
            new OrganizationPosition
            {
                Name = "Accountant",
                Path = HierarchyId.Parse("/4/1/1/1/1/")
            },
            new OrganizationPosition
            {
                Name = "Head of Budgeting",
                Path = HierarchyId.Parse("/4/1/2/")
            },
            new OrganizationPosition
            {
                Name = "Finance Analyst",
                Path = HierarchyId.Parse("/4/1/2/1/")
            },
            new OrganizationPosition
            {
                Name = "Head of Tax Department",
                Path = HierarchyId.Parse("/4/2/")
            },
            new OrganizationPosition
            {
                Name = "Head of Reporting Department",
                Path = HierarchyId.Parse("/4/2/")
            }
        };

        for (int i = 0; i < data.Count; i++)
        {
            data[i].Id = -(i + 1);
        }

        modelBuilder.Entity<OrganizationPosition>().HasData(data);
    }
}