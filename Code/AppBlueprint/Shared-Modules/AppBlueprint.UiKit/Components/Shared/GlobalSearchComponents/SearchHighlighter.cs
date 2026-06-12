using System.Net;

namespace AppBlueprint.UiKit.Components.Shared.GlobalSearchComponents;

/// <summary>
/// Builds the highlighted-title HTML used by the global search results.
/// SECURITY: the title and query originate from user/data input and are rendered as a
/// <c>MarkupString</c>, so every dynamic segment MUST be HTML-encoded before the <c>&lt;mark&gt;</c>
/// markup is inserted. Only the static highlight wrapper is trusted HTML.
/// </summary>
public static class SearchHighlighter
{
    private const string MarkOpen = "<mark class=\"bg-yellow-200 dark:bg-yellow-500/30 text-inherit rounded px-0.5\">";
    private const string MarkClose = "</mark>";

    /// <summary>
    /// Returns HTML where the first case-insensitive occurrence of <paramref name="query"/> in
    /// <paramref name="text"/> is wrapped in a highlight span. All user-supplied text is HTML-encoded.
    /// </summary>
    public static string Highlight(string text, string query)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (string.IsNullOrEmpty(query))
        {
            return WebUtility.HtmlEncode(text);
        }

        int index = text.IndexOf(query, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return WebUtility.HtmlEncode(text);
        }

        string before = WebUtility.HtmlEncode(text[..index]);
        string match = WebUtility.HtmlEncode(text.Substring(index, query.Length));
        string after = WebUtility.HtmlEncode(text[(index + query.Length)..]);

        return $"{before}{MarkOpen}{match}{MarkClose}{after}";
    }
}
