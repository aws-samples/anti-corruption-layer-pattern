// See https://aka.ms/new-console-template for more information

using System.Net;
using anti_corruption_layer.Models;
using anti_corruption_layer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<IACL, UserServiceACL>();
        services.AddSingleton<IUserInMonolith, UserInMonolith>();
    })
    .Build();

UpdateUserinMonolith();
host.Run();

void UpdateUserinMonolith()
{
    var userDetails = new UserDetails(userId: 12345, city: "San Francisco", country: "Unites States",
        state: "California", addressLine1: "475 Sansome St", addressLine2: "10th floor", zipCode: "94111");
    var testACL = host.Services.GetRequiredService<IUserInMonolith>();
    var statusCode = testACL.UpdateAddress(userDetails).Result;
    Console.WriteLine("Return status:" + statusCode);
}