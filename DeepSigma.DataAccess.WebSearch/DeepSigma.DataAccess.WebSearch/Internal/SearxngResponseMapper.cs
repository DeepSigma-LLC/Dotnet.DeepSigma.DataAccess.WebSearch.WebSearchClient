using DeepSigma.DataAccess.WebSearch.Internal.Dto;
using DeepSigma.DataAccess.WebSearch.Models;

namespace DeepSigma.DataAccess.WebSearch.Internal;

/// <summary>
/// Maps an internal <see cref="SearxngJsonResponse"/> DTO to the public
/// <see cref="SearchResponse"/> domain model.
/// </summary>
/// <remarks>
/// Keeping this mapping logic in a dedicated static class makes it independently testable
/// without requiring an HTTP layer or a live SearXNG instance.
/// </remarks>
internal static class SearxngResponseMapper
{
    /// <summary>
    /// Converts a deserialized SearXNG JSON response into a <see cref="SearchResponse"/>.
    /// </summary>
    /// <param name="dto">The deserialized JSON response from the SearXNG instance.</param>
    /// <param name="request">
    /// The original search request, used to populate <see cref="SearchMetadata.Query"/>
    /// and <see cref="SearchMetadata.Page"/>.
    /// </param>
    /// <param name="baseUri">
    /// The base URI of the SearXNG instance, recorded in <see cref="SearchMetadata.InstanceBaseUrl"/>.
    /// </param>
    /// <param name="duration">
    /// The elapsed time of the full HTTP round trip, recorded in <see cref="SearchMetadata.Duration"/>.
    /// </param>
    /// <returns>
    /// A fully populated <see cref="SearchResponse"/> with mapped results and metadata.
    /// Results with a <see langword="null"/> or whitespace <c>url</c> field are excluded.
    /// </returns>
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
