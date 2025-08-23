using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Handlers;

public interface IAsyncHandler: IHttpHandler
{
    public Task HandleRequestAsync(HttpRequest request, Socket socket);
}