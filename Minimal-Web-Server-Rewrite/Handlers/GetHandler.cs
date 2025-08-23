using System.Net.Sockets;
using System.Text;
using Minimal_Web_Server_Rewrite.Models;
using Minimal_Web_Server_Rewrite.ResponsePipeline;
using Minimal_Web_Server_Rewrite.ResponsePipeline.Middlewares;

namespace Minimal_Web_Server_Rewrite.Handlers;

public class GetHandler: IAsyncHandler
{
    public async Task HandleRequestAsync(HttpRequest request, Socket socket)
    {
        var response = new HttpResponse.ResponseBuilder()
            .WithStatusCode(HttpStatusCode.Ok)
            .WithStatusText("OK")
            .WithStringBody("hello world")
            .Build();
        
        Console.WriteLine(response.ToString());
        var pipeline = new ResponsePipeline.ResponsePipeline();
        var middleware = new LoggingMiddleware();
        pipeline.Use(middleware);
        await pipeline.ExecutePipelineAsync(request, response, socket);
    }
}