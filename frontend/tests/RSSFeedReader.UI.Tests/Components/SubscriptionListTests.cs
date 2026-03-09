using Bunit;
using FluentAssertions;
using RSSFeedReader.UI.Components;
using RSSFeedReader.UI.Models;

namespace RSSFeedReader.UI.Tests.Components;

public class SubscriptionListTests : TestContext
{
    [Fact]
    public void SubscriptionList_RendersEmptyMessageWhenNoSubscriptions()
    {
        var cut = RenderComponent<SubscriptionList>(parameters =>
            parameters.Add(p => p.Subscriptions, Enumerable.Empty<SubscriptionDto>()));

        cut.Find("[data-testid='subscriptions-empty']").TextContent.Should().Contain("No subscriptions");
    }

    [Fact]
    public void SubscriptionList_RendersAllSubscriptionsWhenProvided()
    {
        var data = new[]
        {
            new SubscriptionDto { Id = 1, Url = "https://example.com/1", AddedAt = DateTime.UtcNow },
            new SubscriptionDto { Id = 2, Url = "https://example.com/2", AddedAt = DateTime.UtcNow.AddMinutes(1) }
        };

        var cut = RenderComponent<SubscriptionList>(parameters =>
            parameters.Add(p => p.Subscriptions, data));

        var items = cut.FindAll("[data-testid='subscription-item']");
        items.Should().HaveCount(2);
        cut.Markup.Should().Contain("https://example.com/1");
        cut.Markup.Should().Contain("https://example.com/2");
    }

    [Fact]
    public void SubscriptionList_RendersSubscriptionsInAscendingAddedAtOrder()
    {
        var later = new SubscriptionDto { Id = 2, Url = "https://example.com/later", AddedAt = new DateTime(2026, 3, 8, 10, 0, 0, DateTimeKind.Utc) };
        var earlier = new SubscriptionDto { Id = 1, Url = "https://example.com/earlier", AddedAt = new DateTime(2026, 3, 8, 9, 0, 0, DateTimeKind.Utc) };

        var cut = RenderComponent<SubscriptionList>(parameters =>
            parameters.Add(p => p.Subscriptions, new[] { later, earlier }));

        var items = cut.FindAll("[data-testid='subscription-item']");
        items[0].TextContent.Should().Contain("https://example.com/earlier");
        items[1].TextContent.Should().Contain("https://example.com/later");
    }
}
