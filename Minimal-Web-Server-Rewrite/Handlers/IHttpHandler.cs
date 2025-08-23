using System.Net.Sockets;
using System.Text;
using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Handlers;

public interface IHttpHandler
{
    public void HandleRequest(HttpRequest request, Socket socket)
    {
        var response = new HttpResponse.ResponseBuilder()
            .WithStatusCode(HttpStatusCode.BadRequest) // not implemented can be added later
            .WithStatusText("Bad Request")
            .Build();
        socket.Send(Encoding.UTF8.GetBytes(response.ToString()));
    }

    public Task HandleRequestAsync(HttpRequest request, Socket socket);
}