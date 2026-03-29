using DeepSigma.DataAccess.WebSearch.Models;

namespace DeepSigma.DataAccess.WebSearch;

public interface ISearxngClient
{
    Task<SearchResponse> SearchAsync(
        SearchRequest request,
        CancellationToken cancellationToken = default);
}
