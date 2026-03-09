using RSSFeedReader.API.Models;

namespace RSSFeedReader.API.Services;

/// <summary>
/// In-memory repository used for MVP subscription storage.
/// Data is only kept for the current process lifetime.
/// </summary>
public class InMemorySubscriptionRepository : ISubscriptionRepository
{
    private static readonly List<Subscription> Subscriptions = [];
    private static int _nextId = 1;

    public Task<IEnumerable<Subscription>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Subscription>>(Subscriptions.ToList());
    }

    public Task<Subscription> AddAsync(string url)
    {
        var subscription = new Subscription
        {
            Id = _nextId++,
            Url = url,
            AddedAt = DateTime.UtcNow
        };

        Subscriptions.Add(subscription);
        return Task.FromResult(subscription);
    }
}
