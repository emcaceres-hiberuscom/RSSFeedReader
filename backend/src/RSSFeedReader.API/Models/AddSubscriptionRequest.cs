using System.ComponentModel.DataAnnotations;

namespace RSSFeedReader.API.Models;

/// <summary>
/// Request payload for adding a subscription.
/// </summary>
public class AddSubscriptionRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Url { get; set; } = string.Empty;
}
