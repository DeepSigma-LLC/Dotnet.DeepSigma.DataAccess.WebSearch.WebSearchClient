using DeepSigma.DataAccess.WebSearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using DeepSigma.DataAccess.WebSearch.WebSearchClient;
using DeepSigma.DataAccess.WebSearch.WebSearchClient.Abstraction;
using DeepSigma.DataAccess.WebSearch.UrlRetriever;

var services = new ServiceCollection();

Console.WriteLine("Enter search query:");
string? search = Console.ReadLine();

services.AddLogging(b => b.AddConsole());

services.AddSearxngClient(new SearxngOptions
{
    BaseUri = new Uri("http://localhost:8080"),
    Timeout = TimeSpan.FromSeconds(10),
    UserAgent = "MyApp/1.0"
});

await using var provider = services.BuildServiceProvider();

ISearxngClient searxng = provider.GetRequiredService<ISearxngClient>();
ILogger logger = provider.GetRequiredService<ILogger<Program>>();
using CancellationTokenSource cts = new();
CancellationToken ct = cts.Token;

try
{
    var response = await searxng.SearchAsync(new SearchRequest(search ?? "unknown"), ct);

    foreach (var result in response.Results)
    {
        Console.WriteLine("__________________________");
        Console.WriteLine($"{result.Engine}: {result.Title} — {result.Url}");
        Console.WriteLine($"Relevance: {result.Score}");
        Console.WriteLine(result.Snippet);
        Console.WriteLine(result.Category is not null ? $"Category: {result.Category}" : "No category");
       

        await WebPageRequester.GetData(result.Url);
        Console.WriteLine("__________________________");

        Console.WriteLine();
    }
       
}
catch (SearxngUnsupportedFormatException)
{
    // JSON is disabled on this SearXNG instance — check search.formats in instance settings
    logger.LogError("SearXNG instance does not support JSON responses");
}
catch (SearxngUnavailableException ex)
{
    logger.LogError(ex, "SearXNG instance is unreachable");
}
catch (SearxngException ex)
{
    // Catch-all for any other SearXNG-specific failure
    logger.LogError(ex, "Search failed");
}