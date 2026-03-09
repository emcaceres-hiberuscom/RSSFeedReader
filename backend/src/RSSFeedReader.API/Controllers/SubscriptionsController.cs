using Microsoft.AspNetCore.Mvc;
using RSSFeedReader.API.Models;
using RSSFeedReader.API.Services;

namespace RSSFeedReader.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly SubscriptionService _subscriptionService;

    public SubscriptionsController(SubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    /// <summary>
    /// Adds a new RSS/Atom subscription URL.
    /// </summary>
    /// <param name="request">Subscription payload with URL.</param>
    /// <returns>The created subscription or a validation error response.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddSubscription([FromBody] AddSubscriptionRequest request)
    {
        var candidateUrl = request.Url?.Trim() ?? string.Empty;
        if (candidateUrl.Length == 0)
        {
            return BadRequest(new { error = "URL cannot be empty" });
        }

        if (candidateUrl.Length > 2000)
        {
            return BadRequest(new { error = "URL cannot exceed 2000 characters" });
        }

        var created = await _subscriptionService.AddAsync(candidateUrl);
        var dto = new SubscriptionDto
        {
            Id = created.Id,
            Url = created.Url,
            AddedAt = created.AddedAt
        };

        return StatusCode(StatusCodes.Status201Created, dto);
    }

    /// <summary>
    /// Gets all subscriptions currently stored for the session.
    /// </summary>
    /// <returns>All subscriptions ordered by when they were added.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(SubscriptionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSubscriptions()
    {
        var subscriptions = await _subscriptionService.GetAllAsync();
        var response = new SubscriptionsResponse
        {
            Subscriptions = subscriptions
                .OrderBy(s => s.AddedAt)
                .Select(s => new SubscriptionDto
                {
                    Id = s.Id,
                    Url = s.Url,
                    AddedAt = s.AddedAt
                })
                .ToList()
        };

        return Ok(response);
    }
}
