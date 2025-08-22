using System.Text;
using Minimal_Web_Server_Rewrite.Models;
using HttpMethod = Minimal_Web_Server_Rewrite.Models.HttpMethod;

namespace Minimal_Web_Server_Rewrite.Parsing;

public class HttpParser
{
    public HttpRequest ParseRequest(byte[] buffer)
    {
        var request = new HttpRequest();
        var requestText = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        var lines = requestText.Split("\r\n");;
        if (lines.Length == 0 || string.IsNullOrWhiteSpace(lines[0]))
        {
            return request;
        }
        
        var requestLine = lines[0];
        ParseRequestLine(request, requestLine);
        request.Headers = ParseHeaders(lines, out int bodyIdx);
        
        string headersPart = string.Join("\r\n", lines.Take(bodyIdx));
        byte[] headersPartBytes = Encoding.UTF8.GetBytes(headersPart + "\r\n");
        var bodyStartByteIdx = headersPartBytes.Length;
        
        if (request.Headers.TryGetValue("Content-Length", out var contentLengthStr))
        {
            if (int.TryParse(contentLengthStr, out int contentLength))
            {
                request.Body = ParseBody(buffer, bodyStartByteIdx, contentLength);
            }
        }
        return request;
    }

    private void ParseRequestLine(HttpRequest request, string requestLine)
    {
        var requestParts = requestLine.Split(' ', 3);
        if (requestParts.Length >= 3)
        {
            if (Enum.TryParse<HttpMethod>(requestParts[0], true, out var method))
            {
                request.Method = method;
            }
            request.Path = requestParts[1];
            request.Version = requestParts[2];
        }
    }
    private Dictionary<string, string> ParseHeaders(string[] lines, out int bodyIdx)
    {
        var headers = new Dictionary<string, string>();
        for (var i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                bodyIdx = i + 1;
                return headers;
            }
            var keyValue = line.Split(':');
            if (keyValue.Length == 2)
            {
                headers[keyValue[0].Trim()] =  keyValue[1].Trim();
            }
        }
        bodyIdx = -1;
        return headers;
    }

    private byte[] ParseBody(byte[] buffer, int bodyStartByteIdx, int contentLength)
    {
        if(bodyStartByteIdx < buffer.Length)
        {
            int neededBytes = buffer.Length - bodyStartByteIdx;
            int bytesToRead = Math.Min(neededBytes, contentLength);
            byte[] body = new byte[bytesToRead];
            Array.Copy(buffer, bodyStartByteIdx, body, 0, bytesToRead);
            return body;
        }

        return [];
    }
}