using System.Net;
using anti_corruption_layer.Models;
using Microsoft.Extensions.DependencyInjection;

namespace anti_corruption_layer.Services;

public interface IACL
{
    Task<HttpStatusCode> CallMicroservice(ISourceObject details);
}