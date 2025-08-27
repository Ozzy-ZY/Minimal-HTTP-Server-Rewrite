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
        var routeKey = $"{method.ToString().ToUpper()}:{path}";
        _routes.Add(routeKey, handler);
    }
    
    public async Task<bool> RouteRequestToHandlerAsync(HttpRequest request, Socket socket)
    {
        var routeKey = $"{request.Method.ToString().ToUpper()}:{request.Path}";
        if (_routes.TryGetValue(routeKey, out var handler))
        {
            HttpResponse response = new HttpResponse.ResponseBuilder()
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
            await pipeline.ExecutePipelineAsync(request, response, socket);
            return true;
        }
        HttpResponse.ResponseBuilder responseBuilder = new();
        var notFoundResponse = responseBuilder
            .WithStatusCode(HttpStatusCode.NotFound)
            .WithStatusText("Not Found")
            .Build();
        await pipeline.ExecutePipelineAsync(request, notFoundResponse, socket);
        return false;
        
    }
}