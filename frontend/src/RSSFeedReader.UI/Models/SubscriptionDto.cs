namespace RSSFeedReader.UI.Models;

public class SubscriptionDto
{
    public int Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public DateTime AddedAt { get; set; }
}
