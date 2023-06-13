using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Json
{
    internal class Program
    {
        static void Main(string[] args)
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
                .RuleFor(e => e.DateOfBirth, f => f.Person.DateOfBirth)
                .RuleFor(e => e.Department, f => f.PickRandom(new List<string> { "IT", "Finance" }))
                .RuleFor(e => e.AddressDetails, f => addressFaker.Generate())
                .RuleFor(e => e.Contacts, f => contactFaker.Generate(f.Random.Number(4)))
                .RuleFor(e => e.BillingAddress, f => addressFaker.Generate())
                .RuleFor(e => e.PrimaryContact, f => contactFaker.Generate())
                .Generate(40);

            var demoContext = new DemoContext();

            // AddData(addressFaker, contactFaker, employees);

            #region Value Converter Json Filtering and update

            //var filtered = demoContext.Employees.Where(e => e.AddressDetails.State == "GA").ToList();

            //var me = demoContext.Employees.First(e => e.FirstName == "Giorgi");
            //me.AddressDetails.State = "NY";

            //demoContext.SaveChanges();

            //demoContext.Entry(me).State = EntityState.Modified;
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
            var filterByContact = demoContext.Employees.FromSql(@$"SELECT * FROM [EFDemo].[dbo].[Employees] e
            CROSS APPLY OPENJSON(e.Contacts)
            WITH (ContactName varchar(50) '$.Name') 
            WHERE ContactName like 'John%'").ToList();

            var list = demoContext.Employees.Where(e => e.PrimaryContact.Rules.MaximumMessagesPerDay > 3).ToList();
            list[0].PrimaryContact.Phone = "1234";
            list[0].PrimaryContact.Rules.AllowCall = false;
            
            demoContext.SaveChanges();

            #endregion
        }

        private static void AddData(Faker<AddressDetails> addressFaker, Faker<Contact> contactFaker, List<Employee> employees)
        {
            using var demoContext = new DemoContext();

            var employee = new Employee
            {
                FirstName = "Giorgi",
                LastName = "Dalakishvili",
                Department = "IT",
                DateOfBirth = new DateTime(1987, 1, 2),
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
                AddressDetails = addressFaker.Generate(),
                BillingAddress = addressFaker.Generate(),
                PrimaryContact = contactFaker.Generate()
            };

            demoContext.Employees.Add(employee);
            demoContext.Employees.AddRange(employees);
            demoContext.SaveChanges();
        }
    }

}