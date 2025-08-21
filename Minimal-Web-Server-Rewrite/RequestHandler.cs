using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite;

public class RequestHandler
{
    public void HandleRequest(Socket socket)
    {
        HttpRequest request = new HttpRequest();
        var size = socket.ReceiveBufferSize;
        var buffer = new byte[size];
        socket.Receive(buffer);
        
    }
}