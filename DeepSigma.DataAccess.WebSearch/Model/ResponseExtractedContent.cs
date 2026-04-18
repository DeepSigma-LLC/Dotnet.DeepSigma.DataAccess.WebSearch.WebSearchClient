
namespace DeepSigma.DataAccess.WebSearch.WebSearchClient.Model;

/// <summary>
/// Represents the response from a content extraction operation, containing the URL of the extracted content, the extracted content itself, and the timestamp of when the content was extracted.
/// </summary>
/// <param name="URL">The URL of the extracted content.</param>
/// <param name="MainText">The main text content extracted from the HTML.</param>
/// <param name="Title">The title of the extracted content.</param>
/// <param name="Byline">The byline or author information extracted from the HTML.</param>
/// <param name="Language">The language of the extracted content.</param>
/// <param name="Summary">A summary of the extracted content.</param>
/// <param name="Snippet">An optional snippet of the extracted content. Defaults to null.</param>
/// <param name="Engine">An optional identifier for the extraction engine used. Defaults to null.</param>
/// <param name="PublishedAt">The timestamp indicating when the content was published.</param>
/// <param name="ExtractedAt">The timestamp indicating when the content was extracted.</param>
/// <param name="ParsedUrls">An optional list of URLs parsed from the extracted content. Defaults to null.</param>
/// <param name="Score">An optional relevance score for the extracted content. Defaults to null.</param>
/// <param name="Category">An optional category or classification for the extracted content. Defaults to null.</param>
/// <param name="PrettyUrl">An optional human-readable version of the URL. Defaults to null.</param>
/// <param name="Template">An optional template or format used for the extracted content. Defaults to null.</param>
/// <param name="Thumbnail">An optional URL to a thumbnail image associated with the extracted content. Defaults to null.</param>
/// <param name="ImageUrl">An optional URL to an image associated with the extracted content. Defaults to null.</param>
/// <param name="Author">An optional author of the extracted content. Defaults to null.</param>
/// <param name="Error">Indicates whether an error occurred during content extraction. Defaults to false.</param>
/// <param name="ErrorMessage">An optional error message providing details about any error that occurred during content extraction. Defaults to null.</param>
public record ResponseExtractedContent(
    string URL,
    string MainText,
    string Title,
    string Byline,
    string Language,
    string Summary,
    string? Snippet,
    string? Engine,
    DateTimeOffset PublishedAt,
    DateTimeOffset ExtractedAt,
    IReadOnlyList<string>? ParsedUrls = null,
    double? Score = null,
    string? Category = null,
    string? PrettyUrl = null,
    string? Template = null,
    string? Thumbnail = null,
    string? ImageUrl = null,
    string? Author = null,
    bool Error = false,
    string[]? ErrorMessage = null
    );
