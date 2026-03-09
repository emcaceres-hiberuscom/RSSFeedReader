using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using RSSFeedReader.UI.Services;

namespace RSSFeedReader.UI.Tests.Services;

public class SubscriptionApiClientTests
{
    [Fact]
    public async Task GetSubscriptionsAsync_CallsGetSubscriptionsEndpoint()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new DelegatingHandlerStub(async request =>
        {
            capturedRequest = request;
            var body = """
                {
                  "subscriptions": [
                    { "id": 1, "url": "https://example.com/feed", "addedAt": "2026-03-08T10:00:00Z" }
                  ]
                }
                """;

            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
        });

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5000/") };
        var client = new SubscriptionApiClient(httpClient, NullLogger<SubscriptionApiClient>.Instance);

        var result = await client.GetSubscriptionsAsync();

        capturedRequest.Should().NotBeNull();
        capturedRequest!.Method.Should().Be(HttpMethod.Get);
        capturedRequest.RequestUri!.AbsolutePath.Should().Be("/api/subscriptions");
        result.Should().ContainSingle();
        result[0].Url.Should().Be("https://example.com/feed");
    }

    [Fact]
    public async Task AddSubscriptionAsync_CallsPostSubscriptionsEndpoint()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new DelegatingHandlerStub(async request =>
        {
            capturedRequest = request;
            var body = """
                {
                  "id": 1,
                  "url": "https://example.com/feed",
                  "addedAt": "2026-03-08T10:00:00Z"
                }
                """;

            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
        });

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5000/") };
        var client = new SubscriptionApiClient(httpClient, NullLogger<SubscriptionApiClient>.Instance);

        var created = await client.AddSubscriptionAsync("https://example.com/feed");

        capturedRequest.Should().NotBeNull();
        capturedRequest!.Method.Should().Be(HttpMethod.Post);
        capturedRequest.RequestUri!.AbsolutePath.Should().Be("/api/subscriptions");
        var requestBody = await capturedRequest.Content!.ReadAsStringAsync();
        requestBody.Should().Contain("https://example.com/feed");
        created.Url.Should().Be("https://example.com/feed");
    }

    private sealed class DelegatingHandlerStub : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public DelegatingHandlerStub(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handler(request);
        }
    }
}
