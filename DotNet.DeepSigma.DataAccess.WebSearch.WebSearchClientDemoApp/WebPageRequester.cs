using DeepSigma.DataAccess.WebPageDataExtraction.Extractors;
using DeepSigma.DataAccess.WebPageDataExtraction.Fetchers;
using Microsoft.Extensions.Options;

namespace DeepSigma.DataAccess.WebSearch.DemoApp;


public class WebPageRequester
{
    public static async Task GetData(string url)
    {
        var options = new WebPageFetcherOptions
        {
            UserAgent = "MyBot/1.0 (+https://mysite.example/bot)",
            Timeout = TimeSpan.FromSeconds(45),
            MaxRetries = 2
        };

        var fetcher = HttpWebPageFetcher.Create(options);
        var extractor = new SmartReaderContentExtractor();

        var page = await fetcher.FetchAsync(url);
        var content = await extractor.ExtractAsync(page);

        Console.WriteLine($"Title   : {content.Title}");
        Console.WriteLine($"Byline  : {content.Byline}");
        Console.WriteLine($"Language: {content.Language}");
        Console.WriteLine($"Date    : {content.PublishedAt}");
        Console.WriteLine(content.MainText);
    }
}