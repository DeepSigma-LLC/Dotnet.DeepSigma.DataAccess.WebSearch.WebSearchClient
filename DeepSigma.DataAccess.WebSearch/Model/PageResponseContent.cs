
namespace DeepSigma.DataAccess.WebSearch.WebSearchClient.Model;

/// <summary>
/// Represents the content of a web page as returned by a fetch operation, including its URL, HTML markup, and the
/// timestamp when it was retrieved.
/// </summary>
/// <param name="URL">The absolute URL of the web page from which the content was fetched. Cannot be null or empty.</param>
/// <param name="HTML">The raw HTML markup of the fetched web page. May be empty if the page has no content.</param>
/// <param name="FetchedAt">The date and time, in UTC, when the page content was retrieved.</param>
/// <param name="Title">An optional title of the web page, if it can be extracted from the HTML. Defaults to null.</param>
/// <param name="Byline">An optional byline or author information extracted from the HTML, if available. Defaults to null.</param>
/// <param name="Excerpt">An optional excerpt or summary extracted from the HTML, if available. Defaults to null.</param>
/// <param name="MainText">The main text content extracted from the HTML. This should contain the primary textual content of the page, excluding navigation, ads, and other non-essential elements.</param>
/// <param name="Language">An optional language code (e.g., "en" for English) indicating the language of the fetched content, if it can be determined. Defaults to null.</param>
/// <param name="Error">Indicates whether an error occurred during the fetch operation. Defaults to false.</param>
/// <param name="ErrorMessage">An optional error message providing details about any error that occurred during the fetch operation. Defaults to null.</param>
public record PageResponseContent(
    string URL,
    string HTML,
    DateTimeOffset FetchedAt,
    string? Title,
    string? Byline,
    string? Excerpt,
    string MainText,
    string? Language,
    bool Error = false,
    string[]? ErrorMessage = null
    );
