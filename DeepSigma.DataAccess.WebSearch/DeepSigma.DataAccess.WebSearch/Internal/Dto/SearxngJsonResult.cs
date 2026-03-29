using System.Text.Json.Serialization;

namespace DeepSigma.DataAccess.WebSearch.Internal.Dto;

internal sealed class SearxngJsonResult
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("engine")]
    public string? Engine { get; set; }
}
