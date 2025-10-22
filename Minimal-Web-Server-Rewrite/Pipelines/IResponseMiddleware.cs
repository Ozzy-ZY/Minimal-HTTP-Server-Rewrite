using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Pipelines;

public interface IResponseMiddleware
{
    Task<HttpResponse> ProcessAsync(HttpRequest request, HttpResponse response, Func<HttpRequest, HttpResponse, Task<HttpResponse>> next);
}