namespace DeepSigma.DataAccess.WebSearch.Models;

public sealed record SearchResult(
    string Title,
    string Url,
    string? Snippet,
    string? Engine,
    IReadOnlyList<string>? ParsedUrls = null);
