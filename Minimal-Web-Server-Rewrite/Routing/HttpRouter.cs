using System.Net.Sockets;
using System.Text;
using Minimal_Web_Server_Rewrite.Handlers;
using Minimal_Web_Server_Rewrite.Models;
using Minimal_Web_Server_Rewrite.Pipelines;
using HttpMethod = Minimal_Web_Server_Rewrite.Models.HttpMethod;

namespace Minimal_Web_Server_Rewrite.Routing;

public class HttpRouter(ResponsePipeline pipeline) : IRouter
{
    private readonly Dictionary<string, IHttpHandler> _routes = new();

    public void AddRoute(HttpMethod method,string path, IHttpHandler handler)
    {
        var routeKey = GetRouteKey(method, path);
        _routes.Add(routeKey, handler);
    }
    
    private static string GetRouteKey(HttpMethod method, string path)
    {
        return $"{method.ToString().ToUpper()}:{path}";
    }
    
    public async Task<bool> RouteRequestToHandlerAsync(HttpRequest request, Socket socket)
    {
        var routeKey = GetRouteKey(request.Method, request.Path);
        HttpResponse response;
        if (_routes.TryGetValue(routeKey, out var handler))
        { 
            response = new HttpResponse.ResponseBuilder()
                .WithStatusCode(HttpStatusCode.NotImplemented)
                .WithStatusText("Not Implemented")
                .Build();
            try
            {
                switch (handler)
                {
                    case IAsyncHandler asyncHandler:
                        response = await asyncHandler.HandleRequestAsync(request, socket);
                        break;
                    case ISyncHandler syncHandler:
                        response = await Task.Run( () => syncHandler.HandleRequest(request, socket));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        else
        {
            HttpResponse.ResponseBuilder responseBuilder = new();
            response = responseBuilder
                .WithStatusCode(HttpStatusCode.NotFound)
                .WithStatusText("Not Found")
                .Build();
        }

        var finalResponse = await pipeline.ExecutePipelineAsync(request, response);
        await pipeline.SendResponseAsync(finalResponse, socket);
    
        return _routes.ContainsKey(routeKey);
        
    }
}