using System.Text;
using Xunit;
using Minimal_Web_Server_Rewrite;
using Minimal_Web_Server_Rewrite.Models;
using HttpMethod = Minimal_Web_Server_Rewrite.Models.HttpMethod;

namespace Minimal_Web_Server_Rewrite.Tests
{
    public class HttpParserTests
    {
        private readonly HttpParser _parser = new HttpParser();

        [Fact]
        public void GivenValidGetRequestWithHeaders_WhenParsingRequest_ThenReturnsCorrectHttpRequest()
        {
            // Arrange
            var requestString = "Get /api/users HTTP/1.1\r\nHost: localhost\r\nUser-Agent: TestAgent\r\n\r\n";
            var buffer = Encoding.UTF8.GetBytes(requestString);

            // Act
            var result = _parser.ParseRequest(buffer);

            // Assert
            Assert.Equal(HttpMethod.Get, result.Method);
            Assert.Equal("/api/users", result.Path);
            Assert.Equal("HTTP/1.1", result.Version);
            Assert.Equal("localhost", result.Headers["Host"]);
            Assert.Equal("TestAgent", result.Headers["User-Agent"]);
        }

        [Fact]
        public void GivenPostRequestWithJsonBody_WhenParsingRequest_ThenReturnsRequestWithCorrectBody()
        {
            // Arrange
            var body = "{\"name\":\"John\",\"age\":30}";
            var requestString = $"POST /api/users HTTP/1.1\r\nContent-Type: application/json\r\nContent-Length: {body.Length}\r\n\r\n{body}";
            var buffer = Encoding.UTF8.GetBytes(requestString);

            // Act
            var result = _parser.ParseRequest(buffer);

            // Assert
            Assert.Equal(HttpMethod.Post, result.Method);
            Assert.Equal("/api/users", result.Path);
            Assert.Equal("application/json", result.Headers["Content-Type"]);
            Assert.Equal(body.Length.ToString(), result.Headers["Content-Length"]);
            Assert.Equal(body, Encoding.UTF8.GetString(result.Body));
        }

        [Fact]
        public void GivenEmptyBuffer_WhenParsingRequest_ThenReturnsDefaultHttpRequest()
        {
            // Arrange
            var buffer = new byte[0];

            // Act
            var result = _parser.ParseRequest(buffer);

            // Assert
            Assert.Equal(HttpMethod.Get, result.Method); // Default value
            Assert.Null(result.Path);
            Assert.Null(result.Version);
            Assert.Empty(result.Headers);
        }

        [Fact]
        public void GivenRequestWithOnlyWhitespace_WhenParsingRequest_ThenReturnsDefaultHttpRequest()
        {
            // Arrange
            var requestString = "   \r\n\r\n   ";
            var buffer = Encoding.UTF8.GetBytes(requestString);

            // Act
            var result = _parser.ParseRequest(buffer);

            // Assert
            Assert.Equal(HttpMethod.Get, result.Method); // Default value
            Assert.Null(result.Path);
            Assert.Null(result.Version);
            Assert.Empty(result.Headers);
        }

        [Fact]
        public void GivenRequestWithMultipleHeaders_WhenParsingRequest_ThenReturnsAllHeadersCorrectly()
        {
            // Arrange
            var requestString = "Get /test HTTP/1.1\r\nHost: Ozzy.com\r\nAccept: application/json\r\nAuthorization: Bearer token123\r\nUser-Agent: Mozilla/5.0\r\n\r\n";
            var buffer = Encoding.UTF8.GetBytes(requestString);

            // Act
            var result = _parser.ParseRequest(buffer);

            // Assert
            Assert.Equal(4, result.Headers.Count);
            Assert.Equal("Ozzy.com", result.Headers["Host"]);
            Assert.Equal("application/json", result.Headers["Accept"]);
            Assert.Equal("Bearer token123", result.Headers["Authorization"]);
            Assert.Equal("Mozilla/5.0", result.Headers["User-Agent"]);
        }

        [Fact]
        public void GivenRequestWithInvalidHttpMethod_WhenParsingRequest_ThenUsesDefaultMethod()
        {
            // Arrange
            var requestString = "INVALID /test HTTP/1.1\r\nHost: Ozzy.com\r\n\r\n";
            var buffer = Encoding.UTF8.GetBytes(requestString);

            // Act
            var result = _parser.ParseRequest(buffer);

            // Assert
            Assert.Equal(HttpMethod.Get, result.Method); // Default when parsing fails
            Assert.Equal("/test", result.Path);
            Assert.Equal("HTTP/1.1", result.Version);
        }

        [Fact]
        public void GivenRequestWithBadHeaders_WhenParsingRequest_ThenIgnoresBadHeaders()
        {
            // Arrange
            var requestString = "Get /test HTTP/1.1\r\nHost: Ozzy.com\r\nBadHeader\r\nValid-Header: value\r\n\r\n";
            var buffer = Encoding.UTF8.GetBytes(requestString);

            // Act
            var result = _parser.ParseRequest(buffer);

            // Assert
            Assert.Equal(2, result.Headers.Count);
            Assert.Equal("Ozzy.com", result.Headers["Host"]);
            Assert.Equal("value", result.Headers["Valid-Header"]);
            Assert.False(result.Headers.ContainsKey("BadHeader"));
        }

        [Fact]
        public void GivenRequestWithIncompleteRequestLine_WhenParsingRequest_ThenHandlesGracefully()
        {
            // Arrange
            var requestString = "Get\r\nHost: Ozzy.com\r\n\r\n";
            var buffer = Encoding.UTF8.GetBytes(requestString);

            // Act
            var result = _parser.ParseRequest(buffer);

            // Assert
            Assert.Equal(HttpMethod.Get, result.Method);
            Assert.Null(result.Path);
            Assert.Null(result.Version);
            Assert.Equal("Ozzy.com", result.Headers["Host"]);
        }

        [Fact]
        public void GivenPutRequestWithLargeBody_WhenParsingRequest_ThenReturnsCorrectBodyLength()
        {
            // Arrange
            var body = new string('A', 1000); // 1000 character body
            var requestString = $"PUT /api/data HTTP/1.1\r\nContent-Length: {body.Length}\r\n\r\n{body}";
            var buffer = Encoding.UTF8.GetBytes(requestString);

            // Act
            var result = _parser.ParseRequest(buffer);

            // Assert
            Assert.Equal(HttpMethod.Put, result.Method);
            Assert.Equal("/api/data", result.Path);
            Assert.Equal(body.Length.ToString(), result.Headers["Content-Length"]);
            Assert.Equal(1000, result.Body.Length);
            Assert.Equal(body, Encoding.UTF8.GetString(result.Body));
        }

        [Fact]
        public void GivenDeleteRequestWithNoHeaders_WhenParsingRequest_ThenReturnsRequestWithEmptyHeaders()
        {
            // Arrange
            var requestString = "DELETE /api/users/123 HTTP/1.1\r\n\r\n";
            var buffer = Encoding.UTF8.GetBytes(requestString);

            // Act
            var result = _parser.ParseRequest(buffer);

            // Assert
            Assert.Equal(HttpMethod.Delete, result.Method);
            Assert.Equal("/api/users/123", result.Path);
            Assert.Equal("HTTP/1.1", result.Version);
            Assert.Empty(result.Headers);
            Assert.Empty(result.Body);
        }
    }
}