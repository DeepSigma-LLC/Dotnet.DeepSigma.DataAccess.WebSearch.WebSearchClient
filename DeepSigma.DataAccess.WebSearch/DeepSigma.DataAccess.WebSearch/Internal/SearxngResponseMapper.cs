using DeepSigma.DataAccess.WebSearch.Internal.Dto;
using DeepSigma.DataAccess.WebSearch.Models;

namespace DeepSigma.DataAccess.WebSearch.Internal;

internal static class SearxngResponseMapper
{
    internal static SearchResponse Map(
        SearxngJsonResponse dto,
        SearchRequest request,
        Uri baseUri,
        TimeSpan duration)
    {
        var results = dto.Results?
            .Where(r => !string.IsNullOrWhiteSpace(r.Url))
            .Select(r => new SearchResult(
                r.Title ?? string.Empty,
                r.Url!,
                r.Content,
                r.Engine))
            .ToList() ?? [];

        return new SearchResponse(
            results,
            new SearchMetadata(
                baseUri.ToString(),
                request.Query,
                request.Page,
                duration,
                Partial: false,
                ResultCount: results.Count),
            []);
    }
}
