using DeepSigma.DataAccess.WebSearch.WebSearchClient.Abstraction;
using Microsoft.Extensions.Logging;

namespace DeepSigma.DataAccess.WebSearch.WebSearchClient;

/// <summary>
/// A client that performs web searches, retrieves URLs, and extracts content from those URLs.
/// </summary>
/// <param name="urlRetriver">The URL retriever used to fetch URLs based on a query.</param>
/// <param name="contentExtractor">The content extractor used to extract content from HTML.</param>
/// <param name="logger">The logger used to log information and errors.</param>
public class WebSearchClient(IUrlRetriver urlRetriver, IContentExtractor contentExtractor, ILogger<WebSearchClient> logger)
{
    readonly ILogger<WebSearchClient> logger = logger;

    readonly IUrlRetriver urlRetriver1 = urlRetriver;

    readonly IContentExtractor contentExtractor = contentExtractor;

    /// <summary>
    /// Searches for the given query, retrieves URLs, and extracts content from each URL.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task SearchAndExtract(string query)
    {
        try
        {
            string[] urls = await urlRetriver1.GetUrls(query);
            foreach (string url in urls)
            {
                logger.LogInformation("Processing URL: {Url}", url);
                try
                {
                    string html = await contentExtractor.FetchContentAsync(url);
                    string content = await contentExtractor.ExtractedContentAsync(html);
                    logger.LogInformation("Extracted content from {Url}: {Content}", url, content);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to extract content from {Url}", url);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve URLs for query: {Query}", query);
        }
    }
}
