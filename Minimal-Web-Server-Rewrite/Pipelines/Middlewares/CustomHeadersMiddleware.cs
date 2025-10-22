using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Pipelines.Middlewares;

public class CustomHeadersMiddleware: IResponseMiddleware
{
    public async Task<HttpResponse> ProcessAsync(
        HttpRequest request, 
        HttpResponse response, 
        Func<HttpRequest, HttpResponse, Task<HttpResponse>> next)
    {
        response.Headers["X-Custom-Header"] = "ServZy/0.2";
        response.Headers["X-Powered-By"] = "ServZy";
        var processedResponse = await next(request, response);
        // after processing the request measure time taken
        processedResponse.Headers["X-Processing-Time"] = DateTime.UtcNow.ToString("s");
        return processedResponse;
    }
}