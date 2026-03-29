namespace DeepSigma.DataAccess.WebSearch.Models;

public sealed record SearchMetadata(
    string InstanceBaseUrl,
    string Query,
    int? Page,
    TimeSpan Duration,
    bool Partial,
    int ResultCount);
