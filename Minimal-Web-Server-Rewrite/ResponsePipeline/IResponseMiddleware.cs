using System.Net.Sockets;
using Minimal_Web_Server_Rewrite.Models;

namespace Minimal_Web_Server_Rewrite.ResponsePipeline;

public interface IResponseMiddleware
{
    Task ProcessAsync(HttpRequest request, ref HttpResponse response, ref LinkedListNode<IResponseMiddleware>? next);
}