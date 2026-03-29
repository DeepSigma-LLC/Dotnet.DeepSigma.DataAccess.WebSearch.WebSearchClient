using DeepSigma.DataAccess.WebSearch.Internal;
using DeepSigma.DataAccess.WebSearch.Internal.Dto;
using DeepSigma.DataAccess.WebSearch.Models;
using Xunit;

namespace DeepSigma.DataAccess.WebSearch.Test;

public class ResponseMappingTests
{
    private static readonly Uri BaseUri = new("http://test.local/");

    [Fact]
    public void Map_WithValidResults_ReturnsCorrectResults()
    {
        var dto = new SearxngJsonResponse
        {
            Results =
            [
                new SearxngJsonResult { Title = "Result 1", Url = "https://example.com/1", Content = "Snippet", Engine = "google" },
                new SearxngJsonResult { Title = "Result 2", Url = "https://example.com/2", Content = null, Engine = "bing" }
            ]
        };

        var response = SearxngResponseMapper.Map(dto, new SearchRequest("query"), BaseUri, TimeSpan.FromMilliseconds(50));

        Assert.Equal(2, response.Results.Count);
        Assert.Equal("Result 1", response.Results[0].Title);
        Assert.Equal("https://example.com/1", response.Results[0].Url);
        Assert.Equal("Snippet", response.Results[0].Snippet);
        Assert.Equal("google", response.Results[0].Engine);
        Assert.Null(response.Results[1].Snippet);
    }

    [Fact]
    public void Map_WithNullResults_ReturnsEmptyList()
    {
        var dto = new SearxngJsonResponse { Results = null };

        var response = SearxngResponseMapper.Map(dto, new SearchRequest("query"), BaseUri, TimeSpan.Zero);

        Assert.Empty(response.Results);
    }

    [Fact]
    public void Map_FiltersMissingAndBlankUrls()
    {
        var dto = new SearxngJsonResponse
        {
            Results =
            [
                new SearxngJsonResult { Title = "Valid", Url = "https://example.com" },
                new SearxngJsonResult { Title = "No URL", Url = null },
                new SearxngJsonResult { Title = "Blank URL", Url = "   " }
            ]
        };

        var response = SearxngResponseMapper.Map(dto, new SearchRequest("query"), BaseUri, TimeSpan.Zero);

        Assert.Single(response.Results);
        Assert.Equal("Valid", response.Results[0].Title);
    }

    [Fact]
    public void Map_PopulatesMetadataCorrectly()
    {
        var dto = new SearxngJsonResponse { Results = [] };
        var duration = TimeSpan.FromMilliseconds(123);

        var response = SearxngResponseMapper.Map(dto, new SearchRequest("my query", Page: 2), BaseUri, duration);

        Assert.Equal("my query", response.Metadata.Query);
        Assert.Equal(2, response.Metadata.Page);
        Assert.Equal(duration, response.Metadata.Duration);
        Assert.Equal(BaseUri.ToString(), response.Metadata.InstanceBaseUrl);
        Assert.Equal(0, response.Metadata.ResultCount);
    }

    [Fact]
    public void Map_UsesEmptyStringForMissingTitle()
    {
        var dto = new SearxngJsonResponse
        {
            Results = [new SearxngJsonResult { Title = null, Url = "https://example.com" }]
        };

        var response = SearxngResponseMapper.Map(dto, new SearchRequest("query"), BaseUri, TimeSpan.Zero);

        Assert.Equal(string.Empty, response.Results[0].Title);
    }
}
