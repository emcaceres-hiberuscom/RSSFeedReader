using System.ComponentModel.DataAnnotations;

namespace RSSFeedReader.UI.Models;

public class AddSubscriptionRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Url { get; set; } = string.Empty;
}
