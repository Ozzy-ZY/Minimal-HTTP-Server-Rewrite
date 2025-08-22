using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Handlers;
using Minimal_Web_Server_Rewrite.Parsing;
using Minimal_Web_Server_Rewrite.Routing;
using HttpMethod = Minimal_Web_Server_Rewrite.Models.HttpMethod;

namespace Minimal_Web_Server_Rewrite;

class Server
{
    static void Main(string[] args)
    {
        var tcpListener = TcpListener.Create(5000);
        tcpListener.Start();
        var router = new HttpRouter();
        var parser = new HttpParser();
        
        router.AddRoute(HttpMethod.Get, "/", new GetHandler());
        router.AddRoute(HttpMethod.Post, "/", new PostHandler());
        Console.WriteLine("Server started");
        while (true)
        {
            Socket socket = tcpListener.AcceptSocket();
            RequestHandler requestHandler = new RequestHandler(parser, router);
            requestHandler.HandleRequest(socket);
            socket.Close();
        }
        Console.WriteLine("Server stopped");
    }
}