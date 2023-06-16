using Bogus;
using Microsoft.EntityFrameworkCore;

namespace TemporalTables
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            GenerateSampleData(50);

            var context = new TemporalSampleContext();
            var persons = context.Persons.TemporalAsOf(DateTime.Now.AddHours(-1)).ToList();

            var addressesOld = context.Addresses.TemporalBetween(DateTime.Today.AddDays(-3), DateTime.Now)
                .OrderBy(a => EF.Property<DateTime>(a, "PeriodStart")).Select(
                    c => new
                    {
                        //Period columns mapped to shadow properties
                        Customer = c,
                        ValidFrom = EF.Property<DateTime>(c, "PeriodStart"),
                        ValidTo = EF.Property<DateTime>(c, "PeriodEnd")
                    })
                .ToList();

            var withContacts = context.Persons.TemporalAsOf(DateTime.Now.AddHours(-1)).Include(p => p.Contacts).ToList();

            DeletePerson(50);

            var deleted = context.Persons.TemporalAll()
                .OrderByDescending(customer => EF.Property<DateTime>(customer, "PeriodEnd")).First(p => p.Id == 50);

            deleted.Id = 0;
            context.Persons.Add(deleted);
            context.SaveChanges();
        }

        private static void DeletePerson(int id)
        {
            using var context = new TemporalSampleContext();
            var person = context.Persons.Include(p => p.Contacts).First(c => c.Id == id);
            context.Persons.Remove(person);
            context.SaveChanges();
        }

        private static void GenerateSampleData(int count)
        {
            var addressFaker = new Faker<Address>().StrictMode(true)
                .RuleFor(e => e.Id, f => 0)
                .RuleFor(a => a.Street, f => f.Address.StreetAddress())
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
                .RuleFor(a => a.State, f => f.Address.StateAbbr());

            var contactFaker = new Faker<Contact>().StrictMode(true)
                    .RuleFor(e => e.Id, f => 0)
                    .RuleFor(c => c.Type, f => f.PickRandom(ContactType.Email, ContactType.Phone))
                    .RuleFor(c => c.Value, (f, c) => c.Type == ContactType.Email ? f.Person.Email : f.Person.Phone);

            var persons = new Faker<Person>().StrictMode(true)
                .RuleFor(e => e.Id, f => 0)
                .RuleFor(e => e.AddressId, f => 0)
                .RuleFor(e => e.FirstName, f => f.Person.FirstName)
                .RuleFor(e => e.LastName, f => f.Person.LastName)
                .RuleFor(e => e.Address, f => addressFaker.Generate())
                .RuleFor(e => e.Contacts, f => contactFaker.Generate(f.Random.Number(4)))
                .Generate(count);

            using var temporalSampleContext = new TemporalSampleContext();
            temporalSampleContext.Persons.AddRange(persons);
            temporalSampleContext.SaveChanges();
        }
    }
}