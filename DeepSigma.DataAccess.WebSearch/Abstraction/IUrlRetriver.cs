
namespace DeepSigma.DataAccess.WebSearch.WebSearchClient.Abstraction;

/// <summary>
/// Defines an interface for retrieving URLs based on a search query.
/// </summary>
public interface IUrlRetriver
{
    /// <summary>
    /// Asynchronously retrieves a list of URLs that match the specified search query.
    /// </summary>
    /// <param name="query">The search query used to filter and identify relevant URLs. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. This allows the operation to be cancelled if needed, such as when a timeout occurs or when the user cancels the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an array of strings with the
    /// matching URLs. The array is empty if no URLs are found.</returns>
    public Task<string[]> GetUrls(string query, CancellationToken cancellationToken = default);
}
