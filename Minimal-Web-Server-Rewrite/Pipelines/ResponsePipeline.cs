using System.Net.Sockets;
using System.Text;
using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Pipelines;

public class ResponsePipeline
{
    private readonly LinkedList<IResponseMiddleware> _pipeline = new LinkedList<IResponseMiddleware>();
    public void Use(IResponseMiddleware middleware)
    {
        _pipeline.AddLast(middleware);
    }

    public async Task ExecutePipelineAsync(HttpRequest request, HttpResponse response, Socket socket)
    {
        var next = _pipeline.First;
        while (next != null)
        {
            var middleware = next.Value;
            var nextMiddleware = next.Next?.Value;
            Console.WriteLine(response);
            await middleware.ProcessAsync(request, ref response, ref next);
            next = next?.Next;
        }
        await socket.SendAsync(Encoding.UTF8.GetBytes(response.ToString()));
    }
}