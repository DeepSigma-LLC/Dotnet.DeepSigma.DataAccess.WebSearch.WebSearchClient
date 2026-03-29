namespace DeepSigma.DataAccess.WebSearch.Models;

public sealed record SearchResponse(
    IReadOnlyList<SearchResult> Results,
    SearchMetadata Metadata,
    IReadOnlyList<SearchWarning> Warnings);
