namespace Json;

public class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Department { get; set; }
    public DateTime DateOfBirth { get; set; }

    public AddressDetails AddressDetails { get; set; }
    public List<Contact> Contacts { get; set; }
}

public class Contact
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class AddressDetails
{
    public string Address { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
}