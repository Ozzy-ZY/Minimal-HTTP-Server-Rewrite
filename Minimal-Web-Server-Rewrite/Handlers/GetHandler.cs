using System.Net.Sockets;
using System.Text;
using Minimal_Web_Server_Rewrite.Models;
using Minimal_Web_Server_Rewrite.Pipelines;
using Minimal_Web_Server_Rewrite.Pipelines.Middlewares;

namespace Minimal_Web_Server_Rewrite.Handlers;

public class GetHandler: IAsyncHandler
{
    public Task<HttpResponse> HandleRequestAsync(HttpRequest request, Socket socket)
    {
        var response = new HttpResponse.ResponseBuilder()
            .WithStatusCode(HttpStatusCode.Ok)
            .WithStatusText("OK")
            .WithStringBody("hello world")
            .Build();
        return Task.FromResult(response);
    }
}