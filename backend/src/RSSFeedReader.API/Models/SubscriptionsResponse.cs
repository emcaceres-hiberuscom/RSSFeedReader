namespace RSSFeedReader.API.Models;

/// <summary>
/// Response wrapper for subscription listings.
/// </summary>
public class SubscriptionsResponse
{
    public IEnumerable<SubscriptionDto> Subscriptions { get; set; } = Array.Empty<SubscriptionDto>();
}
