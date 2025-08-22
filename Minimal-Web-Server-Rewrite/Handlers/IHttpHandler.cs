using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Handlers;

public interface IHttpHandler
{
    public void HandleRequest(HttpRequest request, Socket socket);
}