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
            Array.Resize(ref buffer, bytesRead);
            var request = parser.ParseRequest(buffer);
            await router.RouteRequestToHandlerAsync(request, socket);
        }
        finally
        {
            socket.Close();    
        }
    }
}