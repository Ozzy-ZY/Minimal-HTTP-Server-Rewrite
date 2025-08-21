namespace Minimal_Web_Server_Rewrite.Models;

public class HttpResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string StatusText { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
    public byte[]? Body { get; set; }
}