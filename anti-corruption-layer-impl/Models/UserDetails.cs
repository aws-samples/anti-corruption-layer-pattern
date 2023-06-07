namespace anti_corruption_layer.Models;

public class UserDetails
{
    public int UserId { get; set; } 
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public UserDetails(int userId, string city, string country, string state, string addressLine1, string addressLine2,
        string zipCode) 
    {
        UserId = userId;
        City = city;
        Country = country;
        State = state;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        ZipCode = zipCode;
    }
}