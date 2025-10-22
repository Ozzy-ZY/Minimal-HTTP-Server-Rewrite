using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Pipelines.Middlewares;

public class LoggingMiddleware: IResponseMiddleware
{
    public async Task<HttpResponse> ProcessAsync(HttpRequest request, HttpResponse response, Func<HttpRequest, HttpResponse, Task<HttpResponse>> next)
    {
        var processedResponse = await next(request, response);
        Console.WriteLine($"Info: Request for {request.Method} {request.Path} Served With Status {processedResponse.StatusCode}");
        return processedResponse;
    }
}