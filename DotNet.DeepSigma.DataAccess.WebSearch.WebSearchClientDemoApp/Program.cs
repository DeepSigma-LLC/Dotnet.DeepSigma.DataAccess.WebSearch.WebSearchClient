using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using DeepSigma.DataAccess.WebSearch.WebSearchClient;
using DeepSigma.DataAccess.WebSearch.UrlRetriever;
using DeepSigma.DataAccess.WebSearch.UrlRetriever.Models;
using DeepSigma.DataAccess.WebSearch.Abstraction;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;
using DeepSigma.DataAccess.WebSearch.ContentExtraction.Extensions;

var services = new ServiceCollection();

services.AddLogging(b => b.AddConsole().AddFilter("Polly", LogLevel.Warning));
services.AddSearxngClient(new SearxngOptions
{
    BaseUri = new Uri("http://localhost:8081"),
    Timeout = TimeSpan.FromSeconds(10),
    UserAgent = "MyApp/1.0"
});
services.AddWebPageDataExtraction();
services.AddWebSearchClient<SearchRequestOptions>();

await using var provider = services.BuildServiceProvider();

ILogger logger = provider.GetRequiredService<ILogger<Program>>();
var urlRetriever = provider.GetRequiredService<IUrlRetriever<SearchRequestOptions>>();
var htmlRetriever = provider.GetRequiredService<IHtmlRetriever>();
var contentExtractor = provider.GetRequiredService<IContentExtractor>();
WebSearchClient<SearchRequestOptions> webSearchClient = provider.GetRequiredService<WebSearchClient<SearchRequestOptions>>();

using CancellationTokenSource cts = new();
CancellationToken ct = cts.Token;

SearchRequestOptions searchRequestOptions = new()
{
    Engines = ["duckduckgo", "brave", "bing"],
    Language = "en",
};

Console.WriteLine("Enter search query:");
string? search = Console.ReadLine();

var results = await webSearchClient.SearchAndExtract(search ?? "unknown", searchRequestOptions, cancellationToken: ct);

// Step 1: Retrieve URLs
Console.WriteLine("\n=== Step 1: URL Retrieval ===");

Console.WriteLine($"URLs found: {results?.Count ?? 0}");
foreach (var result in results ?? [])
{
    Console.WriteLine($"  - {result.SourceHtmlContent?.Url}");
}

if (results is null || results.Count == 0)
{
    Console.WriteLine("No URLs returned from SearXNG. Check that the SearXNG instance is running and returning results.");
    Console.ReadLine();
    return;
}


foreach (var content in results ?? [])
{
    Console.WriteLine("__________________________");
    if (content.Error)
    {
        Console.WriteLine($"ERROR: {string.Join("; ", content.ErrorMessage ?? [])}");
    }
    else
    {
        Console.WriteLine($"Title   : {content.Title}");
        Console.WriteLine($"Byline  : {content.Byline}");
        Console.WriteLine($"Language: {content.Language}");
        Console.WriteLine($"Date    : {content.PublishedAt}");
        Console.WriteLine($"Text len: {content.MainText?.Length ?? 0}");
        Console.WriteLine(content.MainText?[..Math.Min(content.MainText.Length, 500)]);
    }
    Console.WriteLine("__________________________");
    Console.WriteLine();
}
Console.ReadLine();