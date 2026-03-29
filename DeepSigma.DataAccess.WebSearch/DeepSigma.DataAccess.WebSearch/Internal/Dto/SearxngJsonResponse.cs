using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepSigma.DataAccess.WebSearch.Internal.Dto;

internal sealed class SearxngJsonResponse
{
    [JsonPropertyName("results")]
    public List<SearxngJsonResult>? Results { get; set; }

    [JsonPropertyName("number_of_results")]
    public long? NumberOfResults { get; set; }

    [JsonPropertyName("answers")]
    public List<string>? Answers { get; set; }

    [JsonPropertyName("corrections")]
    public List<string>? Corrections { get; set; }

    [JsonPropertyName("infoboxes")]
    public List<JsonElement>? Infoboxes { get; set; }
}
