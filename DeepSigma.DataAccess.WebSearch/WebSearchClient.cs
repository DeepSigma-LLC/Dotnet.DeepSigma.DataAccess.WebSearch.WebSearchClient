using DeepSigma.DataAccess.WebSearch.Abstraction;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;
using Microsoft.Extensions.Logging;

namespace DeepSigma.DataAccess.WebSearch.WebSearchClient;

/// <summary>
/// A client that performs web searches, retrieves URLs, and extracts content from those URLs.
/// </summary>
/// <param name="urlRetriver">The URL retriever used to fetch URLs based on a query.</param>
/// <param name="htmlRetriver">The HTML retriever used to fetch the HTML content of a web page.</param>
/// <param name="contentExtractor">The content extractor used to extract content from HTML.</param>
/// <param name="logger">The logger used to log information and errors.</param>
public class WebSearchClient<TSearchOptions>(
    IUrlRetriver<TSearchOptions> urlRetriver,
    IHtmlRetriver htmlRetriver,
    IContentExtractor contentExtractor,
    ILogger<WebSearchClient<TSearchOptions>> logger)
    where TSearchOptions : class
{
    readonly ILogger<WebSearchClient<TSearchOptions>> logger = logger;
    readonly IUrlRetriver<TSearchOptions> urlRetriver = urlRetriver;
    readonly IContentExtractor contentExtractor = contentExtractor;
    readonly IHtmlRetriver htmlRetriver = htmlRetriver;

    /// <summary>
    /// Searches for the given query, retrieves URLs, and extracts content from each URL.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="searchOptions">The search options.</param>
    /// <param name="maxConcurrency">The maximum number of URLs to extract concurrently. Must be greater than zero. Defaults to 8.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A list of ResponseExtractedContent objects containing the extracted content from each URL. The list will not
    /// include entries for URLs where extraction failed or returned null.</returns>
    public async Task<List<ResponseExtractedContent>?> SearchAndExtract(string query, TSearchOptions searchOptions, int maxConcurrency = 8, CancellationToken cancellationToken = default)
    {
        try
        {
            List<ResponseUrlRetrival> response = await urlRetriver.SearchAsync(query, searchOptions, cancellationToken);

            return await ExtractAllFromURLs(
                response.Select(x => x.Url),
                maxConcurrency,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Search and extract was cancelled for query: {Query}", query);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve URLs for query: {Query}", query);
            return null;
        }
    }

    /// <summary>
    /// Asynchronously extracts content from all specified URLs, limiting the number of concurrent extraction
    /// operations.
    /// </summary>
    /// <remarks>The method throttles concurrent extraction tasks to avoid exceeding the specified concurrency
    /// limit. The order of results corresponds to the order of the input URLs, excluding any URLs for which extraction
    /// failed.</remarks>
    /// <param name="urls">A collection of URLs from which to extract content.</param>
    /// <param name="maxConcurrency">The maximum number of extraction operations to run concurrently. Must be greater than zero.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A list of ResponseExtractedContent objects containing the extracted content from each URL. The list will not
    /// include entries for URLs where extraction failed or returned null.</returns>
    private async Task<List<ResponseExtractedContent>> ExtractAllFromURLs(
       IEnumerable<string> urls,
       int maxConcurrency,
       CancellationToken cancellationToken = default)
    {
        using var semaphore = new SemaphoreSlim(maxConcurrency);

        Task<ResponseExtractedContent?>[] tasks = urls
            .Select(url => ExtractSingleUrlWithThrottle(url, semaphore, cancellationToken))
            .ToArray();

        ResponseExtractedContent?[] results = await Task.WhenAll(tasks);

        return results
            .Where(r => r is not null)
            .Select(r => r!)
            .ToList();
    }

    /// <summary>
    /// Extracts content from the specified URL, ensuring that the operation is throttled using the provided semaphore.
    /// </summary>
    /// <remarks>This method acquires the semaphore before starting the extraction and releases it when the
    /// operation completes, ensuring that concurrency limits are respected.</remarks>
    /// <param name="url">The URL from which to extract content. Cannot be null or empty.</param>
    /// <param name="semaphore">A SemaphoreSlim instance used to limit concurrent extraction operations. Must not be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation is canceled if the token is triggered.</param>
    /// <returns>A task that represents the asynchronous extraction operation. The task result contains the extracted content, or
    /// null if extraction fails.</returns>
    private async Task<ResponseExtractedContent?> ExtractSingleUrlWithThrottle(
        string url,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            return await ExtractSingleUrl(url, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Asynchronously extracts structured content from the specified URL.
    /// </summary>
    /// <remarks>If an error occurs during extraction, the returned ResponseExtractedContent will indicate the
    /// error and include an error message. If the operation is canceled, an OperationCanceledException is
    /// thrown.</remarks>
    /// <param name="url">The URL of the web page to extract content from. Must be a valid, absolute URL.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a ResponseExtractedContent object
    /// with the extracted content, or an error result if extraction fails.</returns>
    private async Task<ResponseExtractedContent?> ExtractSingleUrl(string url, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.LogInformation("Processing URL: {Url}", url);

            ResponseHtmlContent responseHtmlContent = await htmlRetriver.FetchContentAsync(url, cancellationToken);
            ResponseExtractedContent content = await contentExtractor.ExtractedContentAsync(responseHtmlContent, cancellationToken);

            logger.LogInformation("Extracted content from {Url}", url);
            return content;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Processing cancelled for URL: {Url}", url);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to extract content from {Url}", url);
            return new ResponseExtractedContent(
                MainText: string.Empty,
                Title: string.Empty,
                Error: true,
                ErrorMessage: [ex.Message]
            );
        }
    }
}
