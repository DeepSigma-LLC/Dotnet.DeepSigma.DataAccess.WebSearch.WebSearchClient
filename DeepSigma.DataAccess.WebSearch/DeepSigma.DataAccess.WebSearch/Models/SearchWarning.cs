namespace DeepSigma.DataAccess.WebSearch.Models;

/// <summary>
/// Represents a non-fatal warning attached to a <see cref="SearchResponse"/>,
/// such as a disabled engine or a degraded upstream provider.
/// </summary>
/// <param name="Message">A human-readable description of the warning condition.</param>
public sealed record SearchWarning(string Message);
