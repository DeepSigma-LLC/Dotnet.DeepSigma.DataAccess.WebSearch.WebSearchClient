using System.Diagnostics;
using System.Net;
using System.Text.Json;
using DeepSigma.DataAccess.WebSearch.Exceptions;
using DeepSigma.DataAccess.WebSearch.Internal;
using DeepSigma.DataAccess.WebSearch.Internal.Dto;
using DeepSigma.DataAccess.WebSearch.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeepSigma.DataAccess.WebSearch;

public sealed class SearxngClient : ISearxngClient
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<SearxngOptions> _options;
    private readonly ILogger<SearxngClient> _logger;

    public SearxngClient(
        HttpClient httpClient,
        IOptions<SearxngOptions> options,
        ILogger<SearxngClient> logger)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
    }

    public async Task<SearchResponse> SearchAsync(
        SearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Query))
            throw new ArgumentException("Query is required.", nameof(request));

        var sw = Stopwatch.StartNew();
        var queryString = SearxngQueryBuilder.Build(request);
        var path = $"{_options.Value.SearchPath}?{queryString}";

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, path);
            using var response = await _httpClient.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new SearxngUnsupportedFormatException(
                    "JSON format may be disabled on this SearXNG instance.");

            if ((int)response.StatusCode is >= 400 and < 500)
                throw new SearxngBadRequestException(
                    $"SearXNG returned {(int)response.StatusCode}.",
                    (int)response.StatusCode);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            SearxngJsonResponse? dto;
            try
            {
                dto = await JsonSerializer.DeserializeAsync(
                    stream,
                    JsonContext.Default.SearxngJsonResponse,
                    cancellationToken);
            }
            catch (JsonException ex)
            {
                throw new SearxngParseException("Failed to deserialize the SearXNG response.", ex);
            }

            if (dto is null)
                throw new SearxngParseException("Response body was empty or could not be parsed.");

            sw.Stop();

            var result = SearxngResponseMapper.Map(dto, request, _httpClient.BaseAddress!, sw.Elapsed);

            _logger.LogInformation(
                "SearXNG search completed in {ElapsedMs} ms with {Count} results",
                sw.ElapsedMilliseconds,
                result.Results.Count);

            return result;
        }
        catch (SearxngException)
        {
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new SearxngTimeoutException("The SearXNG request timed out.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new SearxngUnavailableException(
                "Failed to reach the SearXNG instance.", ex);
        }
    }
}
