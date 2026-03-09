using RSSFeedReader.API.Models;

namespace RSSFeedReader.API.Services;

public interface ISubscriptionRepository
{
    Task<IEnumerable<Subscription>> GetAllAsync();

    Task<Subscription> AddAsync(string url);
}
