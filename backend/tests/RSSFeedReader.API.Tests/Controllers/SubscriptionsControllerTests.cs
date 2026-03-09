using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RSSFeedReader.API.Models;
using RSSFeedReader.API.Services;

namespace RSSFeedReader.API.Tests.Controllers;

public class SubscriptionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SubscriptionsControllerTests(WebApplicationFactory<Program> factory)
    {
        ResetInMemoryRepository();
        _client = factory.CreateClient();
    }

    private static void ResetInMemoryRepository()
    {
        var subscriptionsField = typeof(InMemorySubscriptionRepository)
            .GetField("Subscriptions", BindingFlags.Static | BindingFlags.NonPublic);
        var nextIdField = typeof(InMemorySubscriptionRepository)
            .GetField("_nextId", BindingFlags.Static | BindingFlags.NonPublic);

        if (subscriptionsField?.GetValue(null) is List<Subscription> list)
        {
            list.Clear();
        }

        nextIdField?.SetValue(null, 1);
    }

    [Fact]
    public async Task PostSubscriptions_WithValidUrl_ReturnsCreatedWithDto()
    {
        var request = new AddSubscriptionRequest { Url = "https://example.com/feed" };

        var response = await _client.PostAsJsonAsync("/api/subscriptions", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<SubscriptionDto>();
        dto.Should().NotBeNull();
        dto!.Url.Should().Be("https://example.com/feed");
        dto.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task PostSubscriptions_WithEmptyUrl_ReturnsBadRequest()
    {
        var request = new AddSubscriptionRequest { Url = "   " };

        var response = await _client.PostAsJsonAsync("/api/subscriptions", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostSubscriptions_WithUrlTooLong_ReturnsBadRequest()
    {
        var request = new AddSubscriptionRequest { Url = new string('a', 2001) };

        var response = await _client.PostAsJsonAsync("/api/subscriptions", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSubscriptions_Initially_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/api/subscriptions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<SubscriptionsResponse>();
        payload.Should().NotBeNull();
        payload!.Subscriptions.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSubscriptions_AfterPost_ReturnsCreatedSubscription()
    {
        await _client.PostAsJsonAsync("/api/subscriptions", new AddSubscriptionRequest { Url = "https://example.com/feed" });

        var response = await _client.GetAsync("/api/subscriptions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<SubscriptionsResponse>();
        payload.Should().NotBeNull();
        payload!.Subscriptions.Select(s => s.Url).Should().Contain("https://example.com/feed");
    }

    [Fact]
    public async Task GetSubscriptions_AfterThreePosts_ReturnsAllInOrderAdded()
    {
        var urls = new[]
        {
            "https://example.com/feed-1",
            "https://example.com/feed-2",
            "https://example.com/feed-3"
        };

        foreach (var url in urls)
        {
            var postResponse = await _client.PostAsJsonAsync("/api/subscriptions", new AddSubscriptionRequest { Url = url });
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        var response = await _client.GetAsync("/api/subscriptions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<SubscriptionsResponse>();
        payload.Should().NotBeNull();
        payload!.Subscriptions.Select(s => s.Url).Should().ContainInOrder(urls);
    }
}
