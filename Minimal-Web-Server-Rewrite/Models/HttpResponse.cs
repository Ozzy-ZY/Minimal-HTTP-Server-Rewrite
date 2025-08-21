namespace Minimal_Web_Server_Rewrite.Models;

public class HttpResponse
{
    HttpStatusCode StatusCode { get; set; }
    string StatusText { get; set; }
    Dictionary<string, string>? Headers { get; set; }
    byte[]? Body { get; set; }
}