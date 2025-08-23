using System.Net.Sockets;
using System.Text;
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
    
    public async Task<bool> RouteRequestToHandlerAsync(HttpRequest request, Socket socket)
    {
        var routeKey = $"{request.Method.ToString().ToUpper()}:{request.Path}";
        if (_routes.TryGetValue(routeKey, out var handler))
        {
            try
            {
                switch (handler)
                {
                    case IAsyncHandler asyncHandler:
                        await asyncHandler.HandleRequestAsync(request, socket);
                        break;
                    case ISyncHandler syncHandler:
                        await Task.Run( () => syncHandler.HandleRequest(request, socket));
                        break;
                    default:
                        HttpResponse.ResponseBuilder responseBuilder = new();
                        var response = responseBuilder.WithStatusCode(HttpStatusCode.NotFound).WithStatusText("Not Found").Build();
                        await socket.SendAsync(Encoding.UTF8.GetBytes(response.ToString()));
                        break;
                }
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