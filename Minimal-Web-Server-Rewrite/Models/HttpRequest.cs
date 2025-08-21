namespace Minimal_Web_Server_Rewrite.Models;

public class HttpRequest
{
    public HttpMethod Method { get; set; }
    public string Path { get; set; }
    public string Version { get; set; }
    public Dictionary<string, string>? Headers { get; set; } 
    public byte[]? Body { get; set; }
}