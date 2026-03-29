using System.Text.Json.Serialization;
using DeepSigma.DataAccess.WebSearch.Internal.Dto;

namespace DeepSigma.DataAccess.WebSearch.Internal;

[JsonSerializable(typeof(SearxngJsonResponse))]
internal partial class JsonContext : JsonSerializerContext { }
