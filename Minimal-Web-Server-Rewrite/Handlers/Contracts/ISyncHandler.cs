using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Handlers;

public interface ISyncHandler : IHttpHandler
{
    void HandleRequest(HttpRequest request, Socket socket);
}