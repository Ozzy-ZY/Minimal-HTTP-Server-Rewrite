using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Handlers;
using Minimal_Web_Server_Rewrite.Models;
using HttpMethod = Minimal_Web_Server_Rewrite.Models.HttpMethod;

namespace Minimal_Web_Server_Rewrite.Routing;

public interface IRouter
{
    public void AddRoute(HttpMethod method,string path, IHttpHandler handler);
    public bool RouteRequestToHandler(HttpRequest request, Socket socket);
}