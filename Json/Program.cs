using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Json;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var addressFaker = new Faker<AddressDetails>().StrictMode(true)
            .RuleFor(a => a.Address, f => f.Address.StreetAddress())
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
            .RuleFor(a => a.State, f => f.Address.StateAbbr());

        var contactFaker = new Faker<Contact>().StrictMode(true)
            .RuleFor(c => c.Email, f => f.Person.Email)
            .RuleFor(c => c.Name, f => f.Person.FullName)
            .RuleFor(c => c.Phone, f => f.Person.Phone)
            .RuleFor(c => c.Rules, f => new NotificationRules
            {
                AllowCall = f.Random.Bool(),
                AllowEmail = f.Random.Bool(),
                AllowSms = f.Random.Bool(),
                MaximumMessagesPerDay = f.Random.Number(4),
            });

        var employees = new Faker<Employee>().StrictMode(true)
            .RuleFor(e => e.Id, f => 0)
            .RuleFor(e => e.FirstName, f => f.Person.FirstName)
            .RuleFor(e => e.LastName, f => f.Person.LastName)
            .RuleFor(e => e.DateOfBirth, f => DateTime.SpecifyKind(f.Person.DateOfBirth, DateTimeKind.Utc))
            .RuleFor(e => e.Department, f => f.PickRandom(new List<string> { "IT", "Finance" }))
            .RuleFor(e => e.Contacts, f => contactFaker.Generate(f.Random.Number(4)))
            .RuleFor(e => e.BillingAddress, f => addressFaker.Generate())
            .RuleFor(e => e.PrimaryContact, f => contactFaker.Generate())
            .RuleFor(e => e.Links, f => f.Make(f.Random.Number(5), () => f.Internet.Url()))
            .RuleFor(e => e.ImportantDates, f => f.Make(f.Random.Number(5), () => DateOnly.FromDateTime(f.Date.Between(DateTime.Today.AddYears(-10), DateTime.Today))))
            .Generate(5000);

        //await AddData(addressFaker, contactFaker, employees);

        await SqlServerExample();

        //PostgresServerExample();
    }

    private static async Task SqlServerExample()
    {
        await using var demoContext = new SqlServerDemoContext();

        #region JSON Columns filtering

        var filtered = demoContext.Employees.Where(e => e.BillingAddress.State == "GA");

        var me = demoContext.Employees.First(e => e.FirstName == "Giorgi");

        me.BillingAddress.State = "NY";

        await demoContext.SaveChangesAsync();

        me.BillingAddress.State = "GA";
        me.BillingAddress.PostalCode = "1234";

        await demoContext.SaveChangesAsync();


        var query = demoContext.Employees.Where(e => e.PrimaryContact.Rules.MaximumMessagesPerDay > 3);

        var list = demoContext.Employees.Where(e => e.PrimaryContact.Rules.MaximumMessagesPerDay > 3).ToList();
        list[0].PrimaryContact.Phone = "1234";
        list[0].PrimaryContact.Rules.AllowCall = false;
        await demoContext.SaveChangesAsync();

        #region Nested element access

        // Filtering by nested JSON columns
        var filterByContact = demoContext.Employees.Where(e => e.Contacts.Any(c => c.Name.StartsWith("John")));

        //Accessing nested JSON array
        var firstContacts = demoContext.Employees.Where(e => e.Contacts.Any()).Select(e => new
        {
            e.FirstName,
            e.LastName,
            e.Contacts[0].Name,
            e.Contacts[0].Phone,
            e.Contacts.Count
        });
        #endregion


        #region Primitive Collections

        //EF Core knows that ImportantDates is a list of Dates and uses date specific function in the SQL
        query = demoContext.Employees.Where(e => e.ImportantDates.Any(date => date.Year == 2025));

        //Filter collection by another collection
        var years = new[] { 2020, 2022, 2025 };
        query = demoContext.Employees.Where(e => e.ImportantDates.Any(date => years.Contains(date.Year)));

        //Ordering by collection
        var orderByDate = demoContext.Employees
            .Select(e => new
            {
                employee = e,
                FirstDate = e.ImportantDates.OrderBy(v => v).First(),
            }).OrderBy(p => p.FirstDate);

        //Count of ImportantDates
        var countByDate = demoContext.Employees
            .Select(e => new
            {
                employee = e,
                DateCount = e.ImportantDates.Count,
            }).OrderBy(p => p.DateCount);

        //Update Primitive Collections
        var employeesWithLinks = demoContext.Employees.Where(e => e.Links.Any(l => l.EndsWith(".com"))).ToList();

        employeesWithLinks[0].Links.RemoveAt(0);
        employeesWithLinks[0].Links.Add("https://giorgi.dev");

        await demoContext.SaveChangesAsync();
        #endregion

        #endregion
    }

    private static void PostgresServerExample()
    {
        var demoContext = new PostgresDemoContext();

        #region Filtering and update

        //var filtered = demoContext.Employees.Where(e => e.AddressDetails.State == "GA").ToList();

        //var me = demoContext.Employees.First(e => e.FirstName == "Giorgi");
        //me.AddressDetails.State = "NY";

        //demoContext.SaveChanges();

        //demoContext.Entry(me).Property(e => e.AddressDetails).IsModified = true;
        //demoContext.SaveChanges();

        //me.AddressDetails = addressFaker.Generate();
        //demoContext.SaveChanges();

        #endregion

        #region JSON Columns filtering

        var filtered = demoContext.Employees.Where(e => e.BillingAddress.State == "GA").ToList();

        var me = demoContext.Employees.First(e => e.FirstName == "Giorgi");
        me.BillingAddress.State = "NY";

        demoContext.SaveChanges();

        me.BillingAddress.State = "GA";
        me.BillingAddress.PostalCode = "1234";

        demoContext.SaveChanges();

        //var filterByContact = demoContext.Employees.Where(e => e.Contacts.Any(c => c.Name.StartsWith("John"))).ToList();
        var filterByContact = demoContext.Employees.Where(e => EF.Functions.JsonContains(e.Contacts, @"[{""Name"": ""John Doe""}]")).ToList();

        filterByContact = demoContext.Employees.FromSql($"Select * from \"Employees\" where jsonb_path_exists(\"Employees\".\"Contacts\", '$[*] ? (@.Name ==\"John Doe\")')")
            .ToList();

        var list = demoContext.Employees.Where(e => e.PrimaryContact.Rules.MaximumMessagesPerDay > 3).ToList();
        list[0].PrimaryContact.Phone = "1234";
        list[0].PrimaryContact.Rules.AllowCall = false;

        demoContext.SaveChanges();

        #endregion
    }

    private static async Task AddData(Faker<AddressDetails> addressFaker, Faker<Contact> contactFaker, List<Employee> employees)
    {
        await using var serverDemoContext = new SqlServerDemoContext();

        await serverDemoContext.Database.MigrateAsync();

        var employee = new Employee
        {
            FirstName = "Giorgi",
            LastName = "Dalakishvili",
            Department = "IT",
            DateOfBirth = new DateTime(1987, 1, 2, 0, 0, 0, 0, 0, DateTimeKind.Utc),
            Links = ["https://aboutmycode.com"],
            Contacts =
            [
                new Contact
                {
                    Email = "some@domain.com",
                    Name = "John Doe",
                    Phone = "111 222"
                },

                new Contact
                {
                    Email = "other@domain.com",
                    Name = "Jane Doe",
                    Phone = "333 444"
                }
            ],
            ImportantDates = [new DateOnly(2025, 1, 2)],
            BillingAddress = addressFaker.Generate(),
            PrimaryContact = contactFaker.Generate()
        };

        serverDemoContext.Employees.Add(employee);
        serverDemoContext.Employees.AddRange(employees);
        await serverDemoContext.SaveChangesAsync();


        await using var postgresDemoContext = new PostgresDemoContext();
        await postgresDemoContext.Database.MigrateAsync();

        postgresDemoContext.Employees.Add(employee);
        postgresDemoContext.Employees.AddRange(employees);

        await postgresDemoContext.SaveChangesAsync();
    }
}