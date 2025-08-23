using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Handlers;
using Minimal_Web_Server_Rewrite.Models;
using HttpMethod = Minimal_Web_Server_Rewrite.Models.HttpMethod;

namespace Minimal_Web_Server_Rewrite.Routing;

public class HttpRouter: IRouter
{
    private readonly Dictionary<string, IHttpHandler> _routes = new();
    public void AddRoute(HttpMethod method,string path, IHttpHandler handler)
    {
        var routeKey = $"{method.ToString().ToUpper()}:{path}";
        _routes.Add(routeKey, handler);
    }

    public bool RouteRequestToHandler(HttpRequest request, Socket socket)
    {
        var routeKey = $"{request.Method.ToString().ToUpper()}:{request.Path}";
        if (_routes.TryGetValue(routeKey, out var handler))
        {
            handler.HandleRequest(request, socket);
            return true;
        }

        return false;
    }

    public async Task<bool> RouteRequestToHandlerAsync(HttpRequest request, Socket socket)
    {
        var routeKey = $"{request.Method.ToString().ToUpper()}:{request.Path}";
        if (_routes.TryGetValue(routeKey, out var handler))
        {
            try
            {
                await handler.HandleRequestAsync(request, socket);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return true;
        }

        return false;
        
    }
}