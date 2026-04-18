namespace DeepSigma.DataAccess.WebSearch.WebSearchClient.Model;

/// <summary>
/// Represents the response from a URL retrieval operation, containing an array of URLs and the timestamp of when the URLs were retrieved.
/// </summary>
/// <param name="Url">The URL retrieved from the search query.</param>
/// <param name="Title">The title associated with the URL, if available. Defaults to null.</param>
/// <param name="Snippet">A snippet or summary associated with the URL, if available. Defaults to null.</param>
/// <param name="Engine">An optional identifier for the search engine used to retrieve the URL. Defaults to null.</param>
/// <param name="ParsedUrls">An optional list of URLs parsed from the retrieved URL. Defaults to null.</param>
/// <param name="Engines">An optional list of search engines that contributed to the retrieval of the URL. Defaults to null.</param>
/// <param name="EngineRelevanceScore">An optional relevance score for the retrieved URL. Defaults to null.</param>
/// <param name="Category">An optional category or classification for the retrieved URL. Defaults to null.</param>
/// <param name="PrettyUrl">An optional human-readable version of the URL. Defaults to null.</param>
/// <param name="Template">An optional template or format used for the retrieved URL. Defaults to null.</param>
/// <param name="Thumbnail">An optional URL to a thumbnail image associated with the retrieved URL. Defaults to null.</param>
/// <param name="ImageUrl">An optional URL to an image associated with the retrieved URL. Defaults to null.</param>
/// <param name="Author">An optional author associated with the retrieved URL. Defaults to null.</param>
/// <param name="IframeSrc">An optional source URL for an iframe associated with the retrieved URL. Defaults to null.</param>
/// <param name="PublishedDate">An optional timestamp indicating when the content at the retrieved URL was published. Defaults to null.</param>
/// <param name="RetrievedAt">The timestamp indicating when the URL was retrieved. Defaults to the current UTC time.</param>
public record ResponseUrlRetrival(
    string Url,
    string? Title,
    string? Snippet,
    string? Engine,
    DateTimeOffset RetrievedAt,
    IReadOnlyList<string>? ParsedUrls = null,
    IReadOnlyList<string>? Engines = null,
    double? EngineRelevanceScore = null,
    string? Category = null,
    string? PrettyUrl = null,
    string? Template = null,
    string? Thumbnail = null,
    string? ImageUrl = null,
    string? Author = null,
    string? IframeSrc = null,
    DateTimeOffset? PublishedDate = null
    );
