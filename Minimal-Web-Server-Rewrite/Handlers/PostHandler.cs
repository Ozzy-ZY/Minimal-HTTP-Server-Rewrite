using System.Net.Sockets;
using System.Text;
using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Handlers;

public class PostHandler:IAsyncHandler
{
    public async Task<HttpResponse> HandleRequestAsync(HttpRequest request, Socket socket)
    {
        var responseBuilder = new HttpResponse.ResponseBuilder();
        if (request.Body.Length <= 0)
        {
            var response = responseBuilder
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithStatusText("Bad Request")
                .WithHeaders(request.Headers)
                .Build();
            return response;
        }
        else
        {
            var body = Encoding.UTF8.GetString(request.Body);
            await using var writer = new StreamWriter("data.txt", true);
            await writer.WriteAsync(body);
            var response = responseBuilder
                .WithStatusText("Created")
                .WithStatusCode(HttpStatusCode.Created)
                .Build();
            return response;
        }
    }
}