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
                ;

            var employees = new Faker<Employee>().StrictMode(true)
                .RuleFor(e => e.Id, f => 0)
                .RuleFor(e => e.FirstName, f => f.Person.FirstName)
                .RuleFor(e => e.LastName, f => f.Person.LastName)
                .RuleFor(e => e.DateOfBirth, f => f.Person.DateOfBirth)
                .RuleFor(e => e.Department, f => f.PickRandom(new List<string> { "IT", "Finance" }))
                .RuleFor(e => e.AddressDetails, f => addressFaker.Generate())
                .RuleFor(e => e.Contacts, f => contactFaker.Generate(f.Random.Number(4)))
                .Generate(40);

            var demoContext = new DemoContext();

            AddData(addressFaker, contactFaker, employees);

            #region Value Converter Json Filtering and update

            var filtered = demoContext.Employees.Where(e => e.AddressDetails.State == "GA").ToList();

            var me = demoContext.Employees.First(e => e.FirstName == "Giorgi");
            me.AddressDetails.State = "NY";

            demoContext.SaveChanges();

            demoContext.Entry(me).State = EntityState.Modified;
            demoContext.SaveChanges();

            me.AddressDetails = addressFaker.Generate();
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
            };

            demoContext.Employees.Add(employee);
            demoContext.Employees.AddRange(employees);
            demoContext.SaveChanges();
        }
    }

}