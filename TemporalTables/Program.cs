using Bogus;
using Microsoft.EntityFrameworkCore;

namespace TemporalTables
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            //await GenerateSampleData(50);

            var context = new TemporalSampleContext();

            var persons = await context.Persons.TemporalAsOf(DateTime.Now.AddHours(-10)).OrderBy(p => p.Id).ToListAsync();

            var addressesOld = await context.Addresses.TemporalBetween(DateTime.Today.AddDays(-3), DateTime.Now)
                .OrderBy(a => EF.Property<DateTime>(a, "PeriodStart")).Select(
                    c => new
                    {
                        //Period columns mapped to shadow properties
                        Customer = c,
                        ValidFrom = EF.Property<DateTime>(c, "PeriodStart"),
                        ValidTo = EF.Property<DateTime>(c, "PeriodEnd")
                    })
                .ToListAsync();

            var withAddressNow = await context.Persons.Include(p => p.Address).FirstAsync(p => p.Id == 1);
            
            var withAddress1 = await context.Persons.TemporalAsOf(DateTime.Now.AddHours(-5)).Include(p => p.Address).FirstAsync(p => p.Id == 1);
            
            var withAddress2 = await context.Persons.TemporalAsOf(DateTime.Now.AddHours(-20)).Include(p => p.Address).FirstAsync(p => p.Id == 1);

            var withAddress3 = await context.Persons.TemporalAsOf(DateTime.Now.AddHours(-30)).Include(p => p.Address).FirstAsync(p => p.Id == 1);

            await DeletePerson(1);

            var deleted = await context.Persons.TemporalAll()
                .OrderByDescending(customer => EF.Property<DateTime>(customer, "PeriodEnd")).FirstAsync(p => p.Id == 1);

            deleted.Id = 0;
            context.Persons.Add(deleted);
            await context.SaveChangesAsync();
        }

        private static async Task DeletePerson(int id)
        {
            using var context = new TemporalSampleContext();
            var person = context.Persons.Include(p => p.Contacts).First(c => c.Id == id);
            context.Persons.Remove(person);
            await context.SaveChangesAsync();
        }

        private static async Task GenerateSampleData(int count)
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
            await temporalSampleContext.SaveChangesAsync();
        }
    }
}