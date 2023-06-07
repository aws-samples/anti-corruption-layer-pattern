namespace anti_corruption_layer.Models;

public class UserDetailsWrapped: UserDetails, ISourceObject
{
    public string ServiceName { get; set; }

    public UserDetailsWrapped(string serviceName, UserDetails userDetails) : base(userDetails.UserId, userDetails.City, userDetails.Country, userDetails.State, userDetails.AddressLine1, userDetails.AddressLine2, userDetails.ZipCode)
    {
        this.ServiceName = serviceName;
    }
}