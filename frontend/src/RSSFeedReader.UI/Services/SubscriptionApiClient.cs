using Microsoft.Extensions.Logging;
using RSSFeedReader.UI.Models;
using System.Net.Http.Json;

namespace RSSFeedReader.UI.Services;

/// <summary>
/// API client for subscription management operations.
/// </summary>
public class SubscriptionApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SubscriptionApiClient> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="SubscriptionApiClient"/>.
    /// </summary>
    /// <param name="httpClient">Configured HTTP client using the API base URL.</param>
    /// <param name="logger">Logger for request and error diagnostics.</param>
    public SubscriptionApiClient(HttpClient httpClient, ILogger<SubscriptionApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Gets all subscriptions from the backend API.
    /// </summary>
    /// <returns>Ordered list of subscriptions; empty list if request fails.</returns>
    public async Task<IReadOnlyList<SubscriptionDto>> GetSubscriptionsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/subscriptions");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to load subscriptions. Status: {StatusCode}", response.StatusCode);
                return [];
            }

            var payload = await response.Content.ReadFromJsonAsync<SubscriptionsResponse>();
            return payload?.Subscriptions.OrderBy(s => s.AddedAt).ToList() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while loading subscriptions.");
            return [];
        }
    }

    /// <summary>
    /// Adds a subscription URL through the backend API.
    /// </summary>
    /// <param name="url">Subscription URL to add.</param>
    /// <returns>The created subscription DTO returned by the API.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API request fails.</exception>
    public async Task<SubscriptionDto> AddSubscriptionAsync(string url)
    {
        try
        {
            var request = new AddSubscriptionRequest { Url = url };
            var response = await _httpClient.PostAsJsonAsync("api/subscriptions", request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to add subscription. Status: {StatusCode}", response.StatusCode);
                throw new InvalidOperationException("Unable to add subscription at this time.");
            }

            var created = await response.Content.ReadFromJsonAsync<SubscriptionDto>();
            return created ?? throw new InvalidOperationException("Backend returned an invalid subscription payload.");
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding subscription.");
            throw new InvalidOperationException("Unable to add subscription at this time.", ex);
        }
    }
}
