namespace Minimal_Web_Server_Rewrite.Models;

public class HttpRequest
{
    HttpMethod Method { get; set; }
    string Path { get; set; }
    string Version { get; set; }
    Dictionary<string, string>? Headers { get; set; } 
    byte[]? Body { get; set; }
}