namespace RSSFeedReader.API.Models;

/// <summary>
/// Represents a single RSS/Atom feed subscription.
/// </summary>
public class Subscription
{
    public int Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public DateTime AddedAt { get; set; }
}
