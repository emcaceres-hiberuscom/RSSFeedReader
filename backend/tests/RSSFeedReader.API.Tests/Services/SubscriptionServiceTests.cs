using FluentAssertions;
using Moq;
using RSSFeedReader.API.Models;
using RSSFeedReader.API.Services;

namespace RSSFeedReader.API.Tests.Services;

public class SubscriptionServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsRepositorySubscriptions()
    {
        var expected = new List<Subscription>
        {
            new() { Id = 1, Url = "https://example.com/1", AddedAt = DateTime.UtcNow }
        };
        var repository = new Mock<ISubscriptionRepository>();
        repository.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);
        var service = new SubscriptionService(repository.Object);

        var result = await service.GetAllAsync();

        result.Should().BeEquivalentTo(expected);
        repository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsync_StoresUrlWithoutChangingSpecialChars()
    {
        var input = "https://example.com/feed?category=dev&sort=desc#latest";
        var repository = new Mock<ISubscriptionRepository>();
        repository
            .Setup(r => r.AddAsync(It.IsAny<string>()))
            .ReturnsAsync((string url) => new Subscription { Id = 1, Url = url, AddedAt = DateTime.UtcNow });

        var service = new SubscriptionService(repository.Object);

        var created = await service.AddAsync(input);

        created.Url.Should().Be(input);
        repository.Verify(r => r.AddAsync(input), Times.Once);
    }

    [Fact]
    public async Task AddAsync_AllowsUrlAtMaxLength()
    {
        var input = new string('a', 2000);
        var repository = new Mock<ISubscriptionRepository>();
        repository
            .Setup(r => r.AddAsync(It.IsAny<string>()))
            .ReturnsAsync((string url) => new Subscription { Id = 1, Url = url, AddedAt = DateTime.UtcNow });

        var service = new SubscriptionService(repository.Object);

        var created = await service.AddAsync(input);

        created.Url.Should().Be(input);
        repository.Verify(r => r.AddAsync(input), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ThrowsWhenUrlExceedsMaxLength()
    {
        var input = new string('a', 2001);
        var repository = new Mock<ISubscriptionRepository>();
        var service = new SubscriptionService(repository.Object);

        var action = () => service.AddAsync(input);

        await action.Should().ThrowAsync<ArgumentException>();
        repository.Verify(r => r.AddAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_AllowsWhitespaceOnlyInputAfterTrim()
    {
        var repository = new Mock<ISubscriptionRepository>();
        repository
            .Setup(r => r.AddAsync(It.IsAny<string>()))
            .ReturnsAsync((string url) => new Subscription { Id = 1, Url = url, AddedAt = DateTime.UtcNow });

        var service = new SubscriptionService(repository.Object);

        var created = await service.AddAsync("   ");

        created.Url.Should().BeEmpty();
        repository.Verify(r => r.AddAsync(string.Empty), Times.Once);
    }

    [Fact]
    public async Task AddAsync_MultipleCallsCreateDistinctSubscriptions()
    {
        var repository = new InMemorySubscriptionRepository();
        var service = new SubscriptionService(repository);

        var first = await service.AddAsync("https://example.com/1");
        var second = await service.AddAsync("https://example.com/2");
        var third = await service.AddAsync("https://example.com/3");

        first.Id.Should().NotBe(second.Id);
        second.Id.Should().NotBe(third.Id);
        third.Id.Should().BeGreaterThan(second.Id);
    }
}
