using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Pipelines.Middlewares;

public class LoggingMiddleware: IResponseMiddleware
{
    public Task ProcessAsync(HttpRequest request, ref HttpResponse response, ref LinkedListNode<IResponseMiddleware>? next)
    {
        Console.WriteLine($"Info: Request for {request.Method} {request.Path} Served With Status {response.StatusCode}");
        return Task.CompletedTask;
    }
}