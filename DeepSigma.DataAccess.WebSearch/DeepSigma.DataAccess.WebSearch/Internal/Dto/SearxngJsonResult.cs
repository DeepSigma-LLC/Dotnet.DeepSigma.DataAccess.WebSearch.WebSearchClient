using System.Text.Json.Serialization;

namespace DeepSigma.DataAccess.WebSearch.Internal.Dto;

/// <summary>
/// Internal DTO that represents a single result object within the SearXNG JSON response array.
/// This type is mapped to the public <see cref="Models.SearchResult"/> by
/// <see cref="SearxngResponseMapper"/>.
/// </summary>
internal sealed class SearxngJsonResult
{
    /// <summary>The display title of the result page.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// The canonical URL of the result page.
    /// Results with a <see langword="null"/> or whitespace URL are filtered out during mapping.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// A short excerpt from the result page content.
    /// Mapped to <see cref="Models.SearchResult.Snippet"/>.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>The name of the search engine that returned this result.</summary>
    [JsonPropertyName("engine")]
    public string? Engine { get; set; }
}
