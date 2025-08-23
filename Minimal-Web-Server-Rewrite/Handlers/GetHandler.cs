using System.Net.Sockets;
using System.Text;
using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Handlers;

public class GetHandler: IHttpHandler
{
    public void HandleRequest(HttpRequest request, Socket socket)
    {
        var response = new HttpResponse.ResponseBuilder()
            .WithStatusCode(HttpStatusCode.Ok)
            .WithStatusText("OK")
            .WithStringBody("hello world")
            .Build();
        
        Console.WriteLine(response.ToString());
        socket.Send(Encoding.UTF8.GetBytes(response.ToString()));;
    }

    public async Task HandleRequestAsync(HttpRequest request, Socket socket)
    {
        var response = new HttpResponse.ResponseBuilder()
            .WithStatusCode(HttpStatusCode.Ok)
            .WithStatusText("OK")
            .WithStringBody("hello world")
            .Build();
        
        Console.WriteLine(response.ToString());
        await socket.SendAsync(Encoding.UTF8.GetBytes(response.ToString()));;
    }
}