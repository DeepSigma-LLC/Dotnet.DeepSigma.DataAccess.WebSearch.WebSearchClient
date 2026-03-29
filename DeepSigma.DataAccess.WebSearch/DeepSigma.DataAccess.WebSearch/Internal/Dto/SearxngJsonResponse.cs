using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepSigma.DataAccess.WebSearch.Internal.Dto;

/// <summary>
/// Internal DTO that mirrors the top-level JSON object returned by the SearXNG <c>/search</c> endpoint.
/// This type is mapped to the public <see cref="Models.SearchResponse"/> by
/// <see cref="SearxngResponseMapper"/>.
/// </summary>
internal sealed class SearxngJsonResponse
{
    /// <summary>The list of individual search result objects.</summary>
    [JsonPropertyName("results")]
    public List<SearxngJsonResult>? Results { get; set; }

    /// <summary>
    /// The total number of results reported by SearXNG, which may exceed the number of
    /// items returned in <see cref="Results"/> for the current page.
    /// </summary>
    [JsonPropertyName("number_of_results")]
    public long? NumberOfResults { get; set; }

    /// <summary>Inline answers generated directly by SearXNG or a contributing engine, if any.</summary>
    [JsonPropertyName("answers")]
    public List<string>? Answers { get; set; }

    /// <summary>Spelling corrections or alternative query suggestions returned by SearXNG, if any.</summary>
    [JsonPropertyName("corrections")]
    public List<string>? Corrections { get; set; }

    /// <summary>
    /// Rich infobox objects embedded in the response.
    /// Retained as raw <see cref="JsonElement"/> values for forward compatibility
    /// with future SearXNG infobox schemas.
    /// </summary>
    [JsonPropertyName("infoboxes")]
    public List<JsonElement>? Infoboxes { get; set; }
}
