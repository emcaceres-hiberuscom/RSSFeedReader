using RSSFeedReader.API.Models;

namespace RSSFeedReader.API.Services;

public class SubscriptionService
{
    private readonly ISubscriptionRepository _repository;

    public SubscriptionService(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Subscription>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<Subscription> AddAsync(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        var trimmedUrl = url.Trim();
        if (trimmedUrl.Length > 2000)
        {
            throw new ArgumentException("URL cannot exceed 2000 characters.", nameof(url));
        }

        return await _repository.AddAsync(trimmedUrl);
    }
}
