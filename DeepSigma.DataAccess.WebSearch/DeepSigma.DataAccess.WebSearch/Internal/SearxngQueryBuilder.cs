using System.Globalization;
using DeepSigma.DataAccess.WebSearch.Models;

namespace DeepSigma.DataAccess.WebSearch.Internal;

internal static class SearxngQueryBuilder
{
    internal static string Build(SearchRequest request)
    {
        var parts = new List<string>
        {
            $"q={Uri.EscapeDataString(request.Query)}",
            "format=json"
        };

        if (request.Page is int p)
            parts.Add($"pageno={p.ToString(CultureInfo.InvariantCulture)}");

        if (!string.IsNullOrWhiteSpace(request.Language))
            parts.Add($"language={Uri.EscapeDataString(request.Language)}");

        if (!string.IsNullOrWhiteSpace(request.TimeRange))
            parts.Add($"time_range={Uri.EscapeDataString(request.TimeRange)}");

        if (request.SafeSearch is not null)
            parts.Add($"safesearch={((int)request.SafeSearch.Value).ToString(CultureInfo.InvariantCulture)}");

        if (request.Categories?.Count > 0)
            parts.Add($"categories={Uri.EscapeDataString(string.Join(",", request.Categories))}");

        if (request.Engines?.Count > 0)
            parts.Add($"engines={Uri.EscapeDataString(string.Join(",", request.Engines))}");

        return string.Join("&", parts);
    }
}
