namespace RSSFeedReader.API.Models;

/// <summary>
/// API response model for a subscription.
/// </summary>
public class SubscriptionDto
{
    public int Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public DateTime AddedAt { get; set; }
}
