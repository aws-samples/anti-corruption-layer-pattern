using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using anti_corruption_layer.Models;
using Microsoft.Extensions.Configuration;
namespace anti_corruption_layer.Services;

public class UserServiceACL: IACL
{
    static HttpClient _client = new HttpClient();
    private static string _apiGatewayDev = string.Empty;
    
    public UserServiceACL()
    {
        IConfiguration config = new ConfigurationBuilder().AddJsonFile(AppContext.BaseDirectory + "../../../config.json").Build();
        _apiGatewayDev = config["APIGatewayURL:Dev"];
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
    }
    public async Task<HttpStatusCode> CallMicroservice(ISourceObject details)
    {
        _apiGatewayDev +=  "/" + details.ServiceName;
        Console.WriteLine(_apiGatewayDev);
        
        var userDetails = details as UserDetails;
        var userMicroserviceModel = new UserMicroserviceModel();
        userMicroserviceModel.UserId = userDetails.UserId;
        userMicroserviceModel.Address = userDetails.AddressLine1 + ", " + userDetails.AddressLine2;
        userMicroserviceModel.City = userDetails.City;
        userMicroserviceModel.State = userDetails.State;
        userMicroserviceModel.Country = userDetails.Country;
        
        if (Int32.TryParse(userDetails.ZipCode, out int zipCode))
        {
            userMicroserviceModel.ZipCode = zipCode;
            Console.WriteLine("Updated zip code");
        }
        else
        {
            Console.WriteLine("String could not be parsed.");
            return HttpStatusCode.BadRequest;
        }

        var jsonString = JsonSerializer.Serialize<UserMicroserviceModel>(userMicroserviceModel);
        Console.WriteLine(jsonString);
        
            // Serialize class into JSON
        var payload = JsonSerializer.Serialize(userMicroserviceModel);

        // Wrap our JSON inside a StringContent object
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        Console.WriteLine(content.ToString());        
        //Make the REST API call here to call the Update Address in the User Microservice
        var response = await _client.PostAsync(
            _apiGatewayDev, content);
        return response.StatusCode;
    }  
}