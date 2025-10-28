using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Minimal_Web_Server_Rewrite.Parsing;
using Minimal_Web_Server_Rewrite.Models;
using Minimal_Web_Server_Rewrite.Pipelines;
using Minimal_Web_Server_Rewrite.Pipelines.Middlewares;

namespace Minimal_Web_Server_Rewrite.Tests
{
    public class PerformanceTests
    {
        [Fact]
        public void HttpParser_ShouldReuseInstanceEfficiently()
        {
            // Arrange
            var parser = new HttpParser();
            var requestString = "GET /test HTTP/1.1\r\nHost: localhost\r\n\r\n";
            var buffer = Encoding.UTF8.GetBytes(requestString);

            // Act - Reuse parser multiple times
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                var result = parser.ParseRequest(buffer, buffer.Length);
            }
            sw.Stop();

            // Assert - Should complete quickly with reused instance
            Assert.True(sw.ElapsedMilliseconds < 100, 
                $"Parser should handle 1000 requests quickly but took {sw.ElapsedMilliseconds}ms");
        }

        [Fact]
        public void HttpParser_ParseRequestWithBytesRead_ShouldAvoidBufferCopy()
        {
            // Arrange
            var parser = new HttpParser();
            var requestString = "GET /test HTTP/1.1\r\nHost: localhost\r\n\r\n";
            var buffer = new byte[4096]; // Large buffer
            var actualBytes = Encoding.UTF8.GetBytes(requestString);
            Array.Copy(actualBytes, buffer, actualBytes.Length);

            // Act - Parse with actual bytes read instead of full buffer
            var result = parser.ParseRequest(buffer, actualBytes.Length);

            // Assert - Should parse correctly without copying entire buffer
            Assert.Equal(HttpMethod.Get, result.Method);
            Assert.Equal("/test", result.Path);
            Assert.Equal("HTTP/1.1", result.Version);
        }

        [Fact]
        public async Task ResponsePipeline_ShouldCacheCompiledPipeline()
        {
            // Arrange
            var pipeline = new ResponsePipeline();
            pipeline.Use(new LoggingMiddleware());
            pipeline.Use(new CustomHeadersMiddleware());
            
            var request = new HttpRequest { Method = HttpMethod.Get, Path = "/" };
            var response = new HttpResponse.ResponseBuilder()
                .WithStatusCode(HttpStatusCode.Ok)
                .WithStatusText("OK")
                .Build();

            // Act - Execute pipeline multiple times
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                var result = await pipeline.ExecutePipelineAsync(request, response);
            }
            sw.Stop();

            // Assert - Cached pipeline should be fast
            Assert.True(sw.ElapsedMilliseconds < 200, 
                $"Cached pipeline should handle 1000 executions quickly but took {sw.ElapsedMilliseconds}ms");
        }

        [Fact]
        public void HttpResponse_ToString_ShouldBeEfficient()
        {
            // Arrange
            var response = new HttpResponse.ResponseBuilder()
                .WithStatusCode(HttpStatusCode.Ok)
                .WithStatusText("OK")
                .WithStringBody("Hello World")
                .Build();
            
            response.Headers.Add("X-Custom-Header", "value");

            // Act - Convert to string multiple times
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                var str = response.ToString();
            }
            sw.Stop();

            // Assert - Should be fast with optimized StringBuilder
            Assert.True(sw.ElapsedMilliseconds < 100, 
                $"Response serialization should handle 10000 conversions quickly but took {sw.ElapsedMilliseconds}ms");
        }

        [Fact]
        public void HttpParser_ParseRequestWithLargeBody_ShouldHandleEfficiently()
        {
            // Arrange
            var parser = new HttpParser();
            var body = new string('A', 10000); // 10KB body
            var requestString = $"POST /api/data HTTP/1.1\r\nContent-Type: text/plain\r\nContent-Length: {body.Length}\r\n\r\n{body}";
            var buffer = Encoding.UTF8.GetBytes(requestString);

            // Act
            var sw = Stopwatch.StartNew();
            var result = parser.ParseRequest(buffer, buffer.Length);
            sw.Stop();

            // Assert - Should parse large body efficiently
            Assert.Equal(HttpMethod.Post, result.Method);
            Assert.Equal(10000, result.Body.Length);
            Assert.True(sw.ElapsedMilliseconds < 50, 
                $"Large body parsing should be fast but took {sw.ElapsedMilliseconds}ms");
        }

        [Fact]
        public void HttpResponse_WithMultipleHeaders_ShouldSerializeEfficiently()
        {
            // Arrange
            var builder = new HttpResponse.ResponseBuilder()
                .WithStatusCode(HttpStatusCode.Ok)
                .WithStatusText("OK")
                .WithStringBody("test");
                
            var response = builder.Build();
            
            // Add multiple headers to test capacity optimization
            for (int i = 0; i < 10; i++)
            {
                response.Headers.Add($"X-Header-{i}", $"Value-{i}");
            }

            // Act
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 5000; i++)
            {
                var str = response.ToString();
            }
            sw.Stop();

            // Assert - Should handle multiple headers efficiently
            Assert.True(sw.ElapsedMilliseconds < 100, 
                $"Serialization with multiple headers should be fast but took {sw.ElapsedMilliseconds}ms");
        }
    }
}
