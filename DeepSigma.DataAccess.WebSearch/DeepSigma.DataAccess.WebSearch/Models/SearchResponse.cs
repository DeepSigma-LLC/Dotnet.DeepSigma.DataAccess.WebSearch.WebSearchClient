namespace DeepSigma.DataAccess.WebSearch.Models;

/// <summary>
/// The top-level response returned by <see cref="ISearxngClient.SearchAsync"/>.
/// </summary>
/// <param name="Results">
/// The ordered list of search results. May be empty but is never <see langword="null"/>.
/// </param>
/// <param name="Metadata">
/// Metadata describing the originating request, the responding instance, timing, and result count.
/// </param>
/// <param name="Warnings">
/// Non-fatal warnings generated during the search, such as disabled engines or partial results.
/// May be empty but is never <see langword="null"/>.
/// </param>
public sealed record SearchResponse(
    IReadOnlyList<SearchResult> Results,
    SearchMetadata Metadata,
    IReadOnlyList<SearchWarning> Warnings);
