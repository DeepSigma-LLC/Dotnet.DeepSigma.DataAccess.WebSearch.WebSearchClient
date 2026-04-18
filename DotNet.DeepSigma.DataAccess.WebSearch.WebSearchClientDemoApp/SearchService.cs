using DeepSigma.DataAccess.WebSearch;
using DeepSigma.DataAccess.WebSearch.Models;

namespace DeepSigma.DataAccess.WebSearch.DemoApp;

public class SearchService(ISearxngClient searxng)
{
    public async Task<IReadOnlyList<SearchResult>> FindAsync(string query, CancellationToken ct = default)
    {
        var response = await searxng.SearchAsync(new SearchRequest(query, Language: "en"), ct);
        return response.Results;
    }
}