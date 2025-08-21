using System.Net.Sockets;

namespace Minimal_Web_Server_Rewrite;

class Server
{
    static void Main(string[] args)
    {
        var tcpListener = TcpListener.Create(5000);
        tcpListener.Start();
        Console.WriteLine("Server started");
        while (true)
        {
            Socket socket = tcpListener.AcceptSocket();
            RequestHandler requestHandler = new RequestHandler();
            requestHandler.HandleRequest(socket);
            socket.Close();
        }
        Console.WriteLine("Server stopped");
    }
}