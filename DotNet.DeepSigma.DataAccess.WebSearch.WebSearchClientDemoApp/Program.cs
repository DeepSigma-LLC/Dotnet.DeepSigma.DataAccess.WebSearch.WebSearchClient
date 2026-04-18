using DeepSigma.DataAccess.WebSearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using DeepSigma.DataAccess.WebSearch.WebSearchClient;
using DeepSigma.DataAccess.WebSearch.WebSearchClient.Abstraction;
using DeepSigma.DataAccess.WebSearch.UrlRetriever;
using DeepSigma.DataAccess.WebSearch.WebSearchClient.Model;

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
WebSearchClient webSearchClient = provider.GetRequiredService<WebSearchClient>();

using CancellationTokenSource cts = new();
CancellationToken ct = cts.Token;

List<ResponseExtractedContent>? extractedContents = await webSearchClient.SearchAndExtract(search ?? "unknown", ct);

if(extractedContents == null)
{
    logger.LogWarning("No content was extracted for the query: {Query}", search);
}

foreach (var content in extractedContents ?? Enumerable.Empty<ResponseExtractedContent>())
{
    Console.WriteLine("__________________________");
    Console.WriteLine($"Title   : {content.Title}");
    Console.WriteLine($"Byline  : {content.Byline}");
    Console.WriteLine($"Language: {content.Language}");
    Console.WriteLine($"Date    : {content.PublishedAt}");
    Console.WriteLine(content.MainText);
    Console.WriteLine("__________________________");
    Console.WriteLine();
}