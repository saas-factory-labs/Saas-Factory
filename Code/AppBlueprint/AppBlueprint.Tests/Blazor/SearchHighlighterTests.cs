using AppBlueprint.UiKit.Components.Shared.GlobalSearchComponents;
using FluentAssertions;

namespace AppBlueprint.Tests.Blazor;

internal sealed class SearchHighlighterTests
{
    [Test]
    public async Task Highlight_ShouldEncodeHtmlInTitle_PreventingXss()
    {
        const string maliciousTitle = "<img src=x onerror=alert(1)>";

        string result = SearchHighlighter.Highlight(maliciousTitle, "src");

        // The dangerous markup must be HTML-encoded so it can never form a live tag.
        // Note: inert text like "onerror=alert(1)" legitimately survives encoding; the security
        // property is that the angle brackets are neutralised, not that the words disappear.
        result.Should().NotContain("<img");
        result.Should().Contain("&lt;img");
        await Assert.That(result).Contains("&gt;");
    }

    [Test]
    public async Task Highlight_ShouldEncodeMaliciousQuery()
    {
        const string title = "harmless title";

        string result = SearchHighlighter.Highlight(title, "<script>");

        result.Should().NotContain("<script>");
        await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task Highlight_ShouldWrapMatchInMarkTag()
    {
        string result = SearchHighlighter.Highlight("Hello World", "World");

        result.Should().Contain("<mark");
        result.Should().Contain("World");
        await Assert.That(result).Contains("</mark>");
    }

    [Test]
    public async Task Highlight_WithEmptyQuery_ShouldReturnEncodedTitleWithoutMark()
    {
        string result = SearchHighlighter.Highlight("<b>Title</b>", "");

        result.Should().NotContain("<mark");
        result.Should().Contain("&lt;b&gt;");
        await Assert.That(result).DoesNotContain("<b>");
    }

    [Test]
    public async Task Highlight_WithNoMatch_ShouldReturnEncodedTitle()
    {
        string result = SearchHighlighter.Highlight("Plain", "zzz");

        await Assert.That(result).IsEqualTo("Plain");
    }
}
