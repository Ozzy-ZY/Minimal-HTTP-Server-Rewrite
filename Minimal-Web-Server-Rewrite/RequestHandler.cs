using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Models;
using Minimal_Web_Server_Rewrite.Parsing;
using Minimal_Web_Server_Rewrite.Routing;

namespace Minimal_Web_Server_Rewrite;

public class RequestHandler(HttpParser parser, IRouter router)
{
    public void HandleRequest(Socket socket)
    {
        var size = socket.ReceiveBufferSize;
        var buffer = new byte[size];
        socket.Receive(buffer);
        var request = parser.ParseRequest(buffer);
        router.RouteRequestToHandler(request, socket);
        socket.Close();
    }
}