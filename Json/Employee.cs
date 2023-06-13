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

    public AddressDetails BillingAddress { get; set; }

    public Contact PrimaryContact { get; set; }
}

public class Contact
{
   // public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public NotificationRules Rules { get; set; }
}

public class NotificationRules
{
    public bool AllowEmail { get; set; }
    public bool AllowCall { get; set; }
    public bool AllowSms { get; set; }
    public int MaximumMessagesPerDay { get; set; }
}

public class AddressDetails
{
    public string Address { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    //public List<string> Phones { get; set; }
}