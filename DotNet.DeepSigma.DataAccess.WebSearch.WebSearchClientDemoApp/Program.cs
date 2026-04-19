using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using DeepSigma.DataAccess.WebSearch.WebSearchClient;
using DeepSigma.DataAccess.WebSearch.UrlRetriever;
using DeepSigma.DataAccess.WebSearch.Abstraction;
using DeepSigma.DataAccess.WebSearch.UrlRetriever.Models;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;
using DeepSigma.DataAccess.WebSearch.ContentExtraction.Extensions;

var services = new ServiceCollection();

services.AddLogging(b => b.AddConsole());
services.AddSearxngClient(new SearxngOptions
{
    BaseUri = new Uri("http://localhost:8080"),
    Timeout = TimeSpan.FromSeconds(10),
    UserAgent = "MyApp/1.0"
});
services.AddWebPageDataExtraction();
services.AddSingleton<WebSearchClient<SearchRequestOptions>>();

await using var provider = services.BuildServiceProvider();

ILogger logger = provider.GetRequiredService<ILogger<Program>>();
IUrlRetriever<SearchRequestOptions> urlRetriever = provider.GetRequiredService<IUrlRetriever<SearchRequestOptions>>();
IContentExtractor contentExtractor = provider.GetRequiredService<IContentExtractor>();
IHtmlRetriever htmlRetriever = provider.GetRequiredService<IHtmlRetriever>();
WebSearchClient<SearchRequestOptions> webSearchClient = provider.GetRequiredService<WebSearchClient<SearchRequestOptions>>();

using CancellationTokenSource cts = new();
CancellationToken ct = cts.Token;

SearchRequestOptions searchRequestOptions = new()
{
    Engines = ["google"],
    Language = "en",
    TimeRange = "week"
};

Console.WriteLine("Enter search query:");
string? search = Console.ReadLine();
List<ResponseExtractedContent>? extractedContents = await webSearchClient.SearchAndExtract(search ?? "unknown", searchRequestOptions, cancellationToken: ct);

if(extractedContents == null)
{
    logger.LogWarning("No content was extracted for the query: {Query}", search);
}

foreach (var content in extractedContents ?? [])
{
    Console.WriteLine("__________________________");
    if (content.Error)
    {
        Console.WriteLine($"ERROR: {string.Join("; ", content.ErrorMessage)}");
    }
    else
    {
        Console.WriteLine($"Title   : {content.Title}");
        Console.WriteLine($"Byline  : {content.Byline}");
        Console.WriteLine($"Language: {content.Language}");
        Console.WriteLine($"Date    : {content.PublishedAt}");
        Console.WriteLine(content.MainText);
    }
    Console.WriteLine("__________________________");
    Console.WriteLine();
}