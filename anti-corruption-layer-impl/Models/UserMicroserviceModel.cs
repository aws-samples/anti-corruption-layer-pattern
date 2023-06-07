namespace anti_corruption_layer.Models;

public class UserMicroserviceModel
{
    public int UserId { get; set; }
    public string Address { get; set; } = string.Empty; 
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public int ZipCode { get; set; }
    public string Country { get; set; } = string.Empty;
}