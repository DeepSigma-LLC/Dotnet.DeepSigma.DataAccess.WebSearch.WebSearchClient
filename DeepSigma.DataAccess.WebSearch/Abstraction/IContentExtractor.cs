namespace DeepSigma.DataAccess.WebSearch.WebSearchClient.Abstraction;

/// <summary>
/// Defines an interface for extracting content from HTML. 
/// This interface provides methods for fetching content from a URL and extracting relevant information from the HTML content. 
/// Implementations of this interface can use various techniques to extract meaningful data, such as using libraries like SmartReader or custom parsing logic.
/// </summary>
public interface IContentExtractor
{
    /// <summary>
    /// Asynchronously retrieves the content from the specified URL as a string.
    /// </summary>
    /// <param name="URL">The URL of the resource to fetch. Must be a valid, absolute URI.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. This allows the operation to be cancelled if needed, such as when a timeout occurs or when the user cancels the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the content retrieved from the
    /// specified URL as a string.</returns>
    Task<string> FetchContentAsync(string URL, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously extracts relevant content from the provided HTML string. 
    /// This method processes the HTML and returns a string containing the extracted information, such as the main text, title, byline, language, and publication date. 
    /// The extraction logic can be implemented using various techniques, including parsing libraries or custom algorithms to identify and extract meaningful content from the HTML structure.
    /// </summary>
    /// <param name="html">The HTML content to extract information from.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the extracted content as a string.</returns>
    Task<string> ExtractedContentAsync(string html, CancellationToken cancellationToken = default);
}
