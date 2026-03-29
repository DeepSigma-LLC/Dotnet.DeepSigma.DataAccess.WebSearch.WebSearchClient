namespace DeepSigma.DataAccess.WebSearch.Models;

public sealed record SearchRequest(
    string Query,
    int? Page = null,
    string? Language = null,
    string? TimeRange = null,
    SafeSearchLevel? SafeSearch = null,
    IReadOnlyList<string>? Categories = null,
    IReadOnlyList<string>? Engines = null);
