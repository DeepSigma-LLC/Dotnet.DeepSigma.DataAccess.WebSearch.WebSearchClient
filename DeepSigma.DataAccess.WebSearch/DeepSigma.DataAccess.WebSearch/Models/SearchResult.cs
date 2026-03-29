namespace DeepSigma.DataAccess.WebSearch.Models;

/// <summary>
/// Represents a single normalized search result returned by a SearXNG query.
/// </summary>
/// <param name="Title">The display title of the result page.</param>
/// <param name="Url">The canonical URL of the result page.</param>
/// <param name="Snippet">
/// A short excerpt or description from the result page content, if available.
/// Mapped from the SearXNG <c>content</c> field.
/// </param>
/// <param name="Engine">
/// The name of the search engine that produced this result, if reported by SearXNG.
/// </param>
/// <param name="ParsedUrls">
/// Additional URLs extracted from the result content, if available.
/// </param>
public sealed record SearchResult(
    string Title,
    string Url,
    string? Snippet,
    string? Engine,
    IReadOnlyList<string>? ParsedUrls = null);
