using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Json
{
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
                .RuleFor(c => c.Rules, f => new NotificationRules()
                {
                    AllowCall = f.Random.Bool(),
                    AllowEmail = f.Random.Bool(),
                    AllowSms = f.Random.Bool(),
                    MaximumMessagesPerDay = f.Random.Number(4),
                })
                ;

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
                .Generate(40);

            await SqlServerExample(addressFaker, contactFaker, employees);

            //PostgresServerExample(addressFaker, contactFaker, employees);
        }

        private static async Task SqlServerExample(Faker<AddressDetails> addressFaker, Faker<Contact> contactFaker, List<Employee> employees)
        {
            var demoContext = new SqlServerDemoContext();

            //await AddData(addressFaker, contactFaker, employees);

            #region JSON Columns filtering

            var filtered = demoContext.Employees.Where(e => e.BillingAddress.State == "GA").ToList();

            var me = demoContext.Employees.First(e => e.FirstName == "Giorgi");

            me.BillingAddress.State = "NY";

            await demoContext.SaveChangesAsync();

            me.BillingAddress.State = "GA";
            me.BillingAddress.PostalCode = "1234";

            await demoContext.SaveChangesAsync();

            //New in EF Core 8
            var filterByContact = demoContext.Employees.Where(e => e.Contacts.Any(c => c.Name.StartsWith("John"))).ToList();


            //New in EF Core 8
            var firstContacts = await demoContext.Employees.Where(e => e.Contacts.Any()).Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.Contacts[0].Name,
                e.Contacts[0].Phone,
                e.Contacts.Count
            }).ToListAsync();

            //filterByContact = demoContext.Employees.FromSql(@$"SELECT * FROM [EFDemo].[dbo].[Employees] e
            //CROSS APPLY OPENJSON(e.Contacts)
            //WITH (ContactName varchar(50) '$.Name') 
            //WHERE ContactName like 'John%'").ToList();

            var list = demoContext.Employees.Where(e => e.PrimaryContact.Rules.MaximumMessagesPerDay > 3).ToList();
            list[0].PrimaryContact.Phone = "1234";
            list[0].PrimaryContact.Rules.AllowCall = false;

            await demoContext.SaveChangesAsync();

            //New in EF Core 8
            var employeesWithLinks = demoContext.Employees.Where(e => e.Links.Any(l => l.EndsWith(".com"))).ToList();

            employeesWithLinks[0].Links.RemoveAt(0);
            employeesWithLinks[0].Links.Add("https://giorgi.dev");

            await demoContext.SaveChangesAsync();

            #endregion
        }

        private static void PostgresServerExample(Faker<AddressDetails> addressFaker, Faker<Contact> contactFaker, List<Employee> employees)
        {
            var demoContext = new PostgresDemoContext();

            // AddDataPostgres(addressFaker, contactFaker, employees);

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
            await using var demoContext = new SqlServerDemoContext();

            await demoContext.Database.MigrateAsync();

            var employee = new Employee
            {
                FirstName = "Giorgi",
                LastName = "Dalakishvili",
                Department = "IT",
                DateOfBirth = new DateTime(1987, 1, 2),
                Links = new List<string> { "https://aboutmycode.com" },
                Contacts = new List<Contact>
                {
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
                },
                BillingAddress = addressFaker.Generate(),
                PrimaryContact = contactFaker.Generate()
            };

            demoContext.Employees.Add(employee);
            demoContext.Employees.AddRange(employees);
            await demoContext.SaveChangesAsync();
        }

        private static void AddDataPostgres(Faker<AddressDetails> addressFaker, Faker<Contact> contactFaker, List<Employee> employees)
        {
            using var demoContext = new PostgresDemoContext();

            var employee = new Employee
            {
                FirstName = "Giorgi",
                LastName = "Dalakishvili",
                Department = "IT",
                DateOfBirth = new DateTime(1987, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                Links = new List<string> { "https://giorgi.dev" },
                Contacts = new List<Contact>
                {
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
                },
                BillingAddress = addressFaker.Generate(),
                PrimaryContact = contactFaker.Generate()
            };

            demoContext.Employees.Add(employee);
            demoContext.Employees.AddRange(employees);
            demoContext.SaveChanges();
        }
    }

}