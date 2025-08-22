using System.Text;

namespace Minimal_Web_Server_Rewrite.Models;

public class HttpResponse
{
    public string Version { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string StatusText { get; set; }
    public Dictionary<string, string> Headers { get; set; } = [];
    public byte[] Body { get; set; } = [];

    public class ResponseBuilder
    {
        private HttpResponse response = new HttpResponse();
        public ResponseBuilder WithStatusCode(HttpStatusCode statusCode)
        {
            response.StatusCode = statusCode;
            return this;
        }
        public ResponseBuilder WithStatusText(string statusText)
        {
            response.StatusText = statusText;
            return this;
        }

        public ResponseBuilder WithHeaders(Dictionary<string, string> headers)
        {
            response.Headers = headers;
            return this;
        }

        public ResponseBuilder WithStringBody(string body)
        {
            response.Body = Encoding.UTF8.GetBytes(body);
            response.Headers.Add("Content-Length", response.Body.Length.ToString());
            return this;
        }
        public ResponseBuilder WithBody(byte[] body)
        {
            response.Body = body;
            response.Headers.Add("Content-Length", body.Length.ToString());
            return this;       
        }
        public HttpResponse Build()
        {
            response.Version = "HTTP/1.1";
            return response;
        }
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"{Version} {(int)StatusCode} {StatusText}\r\n");
        foreach (var header in Headers)
        {
            stringBuilder.Append($"{header.Key}: {header.Value}\r\n");
        }
        stringBuilder.Append("\r\n");
        stringBuilder.Append(Encoding.UTF8.GetString(Body));
        return stringBuilder.ToString();
    }
}