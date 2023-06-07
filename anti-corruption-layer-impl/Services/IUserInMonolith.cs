using System.Net;
using anti_corruption_layer.Models;

namespace anti_corruption_layer.Services;

public interface IUserInMonolith
{
    Task<HttpStatusCode> UpdateAddress(UserDetails userDetails);
}