using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Models;
using Minimal_Web_Server_Rewrite.Parsing;
using Minimal_Web_Server_Rewrite.Routing;

namespace Minimal_Web_Server_Rewrite;

public class RequestHandler(HttpParser parser, IRouter router)
{
    public async Task HandleRequestAsync(Socket socket)
    {
        try
        {
            var size = socket.ReceiveBufferSize;
            var buffer = new byte[size];
            int bytesRead = await socket.ReceiveAsync(buffer);
            // Use ArraySegment to avoid copying the entire buffer
            var request = parser.ParseRequest(buffer, bytesRead);
            await router.RouteRequestToHandlerAsync(request, socket);
        }
        finally
        {
            socket.Close();    
        }
    }
}