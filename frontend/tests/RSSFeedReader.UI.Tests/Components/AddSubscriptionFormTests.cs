using Bunit;
using FluentAssertions;
using RSSFeedReader.UI.Components;

namespace RSSFeedReader.UI.Tests.Components;

public class AddSubscriptionFormTests : TestContext
{
    [Fact]
    public void AddSubscriptionForm_RejectsEmptyInputWithValidationError()
    {
        var cut = RenderComponent<AddSubscriptionForm>();

        cut.Find("form").Submit();

        cut.Find("[data-testid='add-subscription-error']").TextContent.Should().Contain("URL cannot be empty");
    }

    [Fact]
    public void AddSubscriptionForm_RejectsInputLongerThan2000Characters()
    {
        var cut = RenderComponent<AddSubscriptionForm>();

        cut.Find("#subscription-url").Change(new string('a', 2001));
        cut.Find("form").Submit();

        cut.Find("[data-testid='add-subscription-error']").TextContent.Should().Contain("URL cannot exceed 2000 characters");
    }

    [Fact]
    public void AddSubscriptionForm_AcceptsValidInputAndFiresCallback()
    {
        string? emittedUrl = null;
        var cut = RenderComponent<AddSubscriptionForm>(parameters =>
            parameters.Add(p => p.OnSubscriptionAdded, (string url) => emittedUrl = url));

        cut.Find("#subscription-url").Change("https://example.com/feed");
        cut.Find("form").Submit();

        emittedUrl.Should().Be("https://example.com/feed");
    }

    [Fact]
    public void AddSubscriptionForm_ClearsInputAfterSuccessfulSubmission()
    {
        var cut = RenderComponent<AddSubscriptionForm>(parameters =>
            parameters.Add(p => p.OnSubscriptionAdded, (string _) => { }));

        cut.Find("#subscription-url").Change("https://example.com/feed");
        cut.Find("form").Submit();

        cut.Find("#subscription-url").GetAttribute("value").Should().Be(string.Empty);
    }
}
