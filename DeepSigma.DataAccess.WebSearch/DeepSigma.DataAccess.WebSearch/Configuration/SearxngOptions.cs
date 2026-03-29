namespace DeepSigma.DataAccess.WebSearch;

public sealed class SearxngOptions
{
    /// <summary>Absolute base URI of the SearXNG instance, e.g. https://searxng.example.com</summary>
    public required Uri BaseUri { get; init; }

    /// <summary>Per-attempt timeout. Defaults to 10 seconds.</summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>Path to the search endpoint. Defaults to /search.</summary>
    public string SearchPath { get; init; } = "/search";

    /// <summary>Optional User-Agent header value sent with every request.</summary>
    public string? UserAgent { get; init; }

    /// <summary>Probe the instance for JSON-format support at startup.</summary>
    public bool ProbeInstanceOnStartup { get; init; } = false;
}
