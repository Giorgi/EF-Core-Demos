namespace TemporalTables;

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public int AddressId { get; set; }
    public Address Address { get; set; }
    public List<Contact> Contacts { get; set; }
}

public class Contact
{
    public int Id { get; set; }
    public string Value { get; set; }
    public ContactType Type { get; set; }
}

public enum ContactType
{
    Phone,
    Email
}

public class Address
{
    public int Id { get; set; }
    public string Street { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
}