using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Handlers;
using Minimal_Web_Server_Rewrite.Parsing;
using Minimal_Web_Server_Rewrite.Routing;
using Minimal_Web_Server_Rewrite.Pipelines;
using Minimal_Web_Server_Rewrite.Pipelines.Middlewares;
using HttpMethod = Minimal_Web_Server_Rewrite.Models.HttpMethod;

namespace Minimal_Web_Server_Rewrite;

class Server
{
    static async Task Main(string[] args)
    {
        var tcpListener = TcpListener.Create(5000);
        tcpListener.Start();
        var pipeline = new ResponsePipeline();
        pipeline.Use(new LoggingMiddleware());
        
        var router = new HttpRouter(pipeline);
        router.AddRoute(HttpMethod.Get, "/", new GetHandler());
        router.AddRoute(HttpMethod.Post, "/", new PostHandler());
        Console.WriteLine("Server started");
        while (true)
        {
            try
            {
                Socket socket = await tcpListener.AcceptSocketAsync();
                _ = Task.Run(async () =>
                {
                    var parser = new HttpParser(); 
                    RequestHandler requestHandler = new RequestHandler(parser, router);
                    await requestHandler.HandleRequestAsync(socket);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        Console.WriteLine("Server stopped");
    }
}