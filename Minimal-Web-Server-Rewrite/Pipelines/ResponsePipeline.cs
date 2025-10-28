using System.Net.Sockets;
using System.Text;
using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.Pipelines;

public class ResponsePipeline
{
    private readonly List<IResponseMiddleware> _pipeline = new() { };
    private Func<HttpRequest, HttpResponse, Task<HttpResponse>>? _compiledPipeline;

    public void Use(IResponseMiddleware middleware)
    {
        _pipeline.Add(middleware);
        // Invalidate compiled pipeline when middleware is added
        _compiledPipeline = null;
    }

    private Func<HttpRequest, HttpResponse, Task<HttpResponse>> BuildPipeline()
    {
        Func<HttpRequest, HttpResponse, Task<HttpResponse>> pipeline = (req, res) => Task.FromResult(res);
        for (int i = _pipeline.Count - 1; i >= 0; i--)
        {
            var middleware = _pipeline[i];
            var next = pipeline; // Capture current pipeline in closure
            
            pipeline = async (req, res) =>
            {
                return await middleware.ProcessAsync(req, res, next);
            };
        }
        return pipeline;
    }

    public async Task<HttpResponse> ExecutePipelineAsync(HttpRequest request, HttpResponse response)
    {
        // Build pipeline once and cache it
        _compiledPipeline ??= BuildPipeline();
        return await _compiledPipeline(request, response);
    }
    
    public async Task SendResponseAsync(HttpResponse response, Socket socket)
    {
        await socket.SendAsync(Encoding.UTF8.GetBytes(response.ToString()));
    }
}