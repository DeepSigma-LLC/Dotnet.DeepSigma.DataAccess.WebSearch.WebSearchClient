using System.Net;
using DeepSigma.DataAccess.WebSearch.Abstraction;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;
using DeepSigma.DataAccess.WebSearch.WebSearchClient;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DeepSigma.DataAccess.WebSearch.Test;

public class WebSearchClientTests
{
    sealed class TestOptions;

    // ── Fakes ────────────────────────────────────────────────────────────────

    sealed class FakeUrlRetriever(List<ResponseUrlRetrival> results) : IUrlRetriever<TestOptions>
    {
        public Task<List<ResponseUrlRetrival>> SearchAsync(
            string query, TestOptions searchOption, CancellationToken cancellationToken)
            => Task.FromResult(results);
    }

    sealed class ThrowingUrlRetriever : IUrlRetriever<TestOptions>
    {
        public Task<List<ResponseUrlRetrival>> SearchAsync(
            string query, TestOptions searchOption, CancellationToken cancellationToken)
            => throw new HttpRequestException("Backend unreachable");
    }

    sealed class FakeHtmlRetriever(Func<string, ResponseHtmlContent> factory) : IHtmlRetriever
    {
        public Task<ResponseHtmlContent> FetchContentAsync(
            string url, CancellationToken cancellationToken)
            => Task.FromResult(factory(url));

        public Task<ResponseHtmlContent> FetchContentAsync(
            ResponseUrlRetrival responseUrl, CancellationToken cancellationToken)
            => FetchContentAsync(responseUrl.Url, cancellationToken);
    }

    sealed class FakeContentExtractor(
        Func<ResponseHtmlContent, ResponseExtractedContent> factory) : IContentExtractor
    {
        public Task<ResponseExtractedContent> ExtractContentAsync(
            ResponseHtmlContent htmlContent, CancellationToken cancellationToken)
            => Task.FromResult(factory(htmlContent));

        public Task<ResponseExtractedContent> ExtractContentAsync(
            string html, string? url, CancellationToken cancellationToken)
            => throw new NotSupportedException();
    }

    // Throws on the Nth call; all others succeed. Use maxConcurrency=1 for determinism.
    sealed class ThrowingOnNthCallContentExtractor(int failOnCall) : IContentExtractor
    {
        int _calls;

        public Task<ResponseExtractedContent> ExtractContentAsync(
            ResponseHtmlContent htmlContent, CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _calls) == failOnCall)
                throw new InvalidOperationException("Extraction failed");
            return Task.FromResult(GoodContent());
        }

        public Task<ResponseExtractedContent> ExtractContentAsync(
            string html, string? url, CancellationToken cancellationToken)
            => throw new NotSupportedException();
    }

    // ── Model helpers ─────────────────────────────────────────────────────────

    static ResponseUrlRetrival UrlResult(string url) => new(
        Url: url, Title: "", Snippet: "", SearchEngine: "",
        RetrievedAt: DateTimeOffset.UtcNow,
        ParsedUrls: null, Engines: null, EngineRelevanceScore: null,
        Category: null, PrettyUrl: null, Template: null, Thumbnail: null,
        ImageUrl: null, Author: null, IframeSrc: null, PublishedDate: null,
        Error: false, ErrorMessage: []);

    static ResponseHtmlContent HtmlContent(string url) => new(
        Url: url, Html: "<html/>",
        FetchedAt: DateTimeOffset.UtcNow,
        StatusCode: HttpStatusCode.OK,
        ContentType: "", Title: "", Byline: "", Excerpt: "", Language: "",
        SourceUrlRetrival: null!, Error: false, ErrorMessage: []);

    static ResponseExtractedContent GoodContent() => new(
        MainText: "content", Title: "title",
        Language: null, Snippet: null, Byline: null, Summary: null,
        PublishedAt: null, ParsedUrls: null, Category: null, PrettyUrl: null,
        Template: null, Thumbnail: null, ImageUrl: null, Author: null,
        SourceHtmlContent: null!, Error: false, ErrorMessage: []);

    static WebSearchClient<TestOptions> CreateClient(
        IUrlRetriever<TestOptions> urlRetriever,
        IHtmlRetriever htmlRetriever,
        IContentExtractor contentExtractor)
        => new(urlRetriever, htmlRetriever, contentExtractor,
               NullLogger<WebSearchClient<TestOptions>>.Instance);

    // ── Tests ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAndExtract_HappyPath_ReturnsExtractedContentForEachUrl()
    {
        var client = CreateClient(
            new FakeUrlRetriever([UrlResult("https://a.com"), UrlResult("https://b.com")]),
            new FakeHtmlRetriever(HtmlContent),
            new FakeContentExtractor(_ => GoodContent()));

        var result = await client.SearchAndExtract("query", new TestOptions(), cancellationToken: CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.False(r.Error));
    }

    [Fact]
    public async Task SearchAndExtract_UrlRetrieverThrows_ReturnsNull()
    {
        var client = CreateClient(
            new ThrowingUrlRetriever(),
            new FakeHtmlRetriever(HtmlContent),
            new FakeContentExtractor(_ => GoodContent()));

        var result = await client.SearchAndExtract("query", new TestOptions(), cancellationToken: CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task SearchAndExtract_OneExtractionFails_FailedEntryHasErrorAndRemainingSucceed()
    {
        // maxConcurrency=1 makes calls sequential so failOnCall=2 reliably targets the second URL
        var client = CreateClient(
            new FakeUrlRetriever([
                UrlResult("https://a.com"),
                UrlResult("https://b.com"),
                UrlResult("https://c.com")]),
            new FakeHtmlRetriever(HtmlContent),
            new ThrowingOnNthCallContentExtractor(failOnCall: 2));

        var result = await client.SearchAndExtract("query", new TestOptions(), maxConcurrency: 1, cancellationToken: CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result.Count(r => r.Error));
        Assert.Equal(2, result.Count(r => !r.Error));
        Assert.NotEmpty(result.Single(r => r.Error).ErrorMessage);
    }

    [Fact]
    public async Task SearchAndExtract_EmptyUrlList_ReturnsEmptyList()
    {
        var client = CreateClient(
            new FakeUrlRetriever([]),
            new FakeHtmlRetriever(HtmlContent),
            new FakeContentExtractor(_ => GoodContent()));

        var result = await client.SearchAndExtract("query", new TestOptions(), cancellationToken: CancellationToken.None);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAndExtract_CancelledToken_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var client = CreateClient(
            new FakeUrlRetriever([UrlResult("https://a.com")]),
            new FakeHtmlRetriever(HtmlContent),
            new FakeContentExtractor(_ => GoodContent()));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.SearchAndExtract("query", new TestOptions(), cancellationToken: cts.Token));
    }

    [Fact]
    public async Task SearchAndExtract_InvalidMaxConcurrency_ThrowsArgumentOutOfRangeException()
    {
        var client = CreateClient(
            new FakeUrlRetriever([UrlResult("https://a.com")]),
            new FakeHtmlRetriever(HtmlContent),
            new FakeContentExtractor(_ => GoodContent()));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => client.SearchAndExtract("query", new TestOptions(), maxConcurrency: 0, cancellationToken: CancellationToken.None));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => client.SearchAndExtract("query", new TestOptions(), maxConcurrency: -1, cancellationToken: CancellationToken.None));
    }
}
