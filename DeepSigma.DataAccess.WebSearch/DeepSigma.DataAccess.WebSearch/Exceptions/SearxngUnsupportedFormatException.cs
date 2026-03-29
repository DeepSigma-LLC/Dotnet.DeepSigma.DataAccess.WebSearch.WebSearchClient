namespace DeepSigma.DataAccess.WebSearch.Exceptions;

/// <summary>
/// The instance returned 403, indicating JSON format is disabled on this SearXNG instance.
/// </summary>
public sealed class SearxngUnsupportedFormatException : SearxngException
{
    public SearxngUnsupportedFormatException(string message) : base(message) { }

    public SearxngUnsupportedFormatException(string message, Exception innerException)
        : base(message, innerException) { }
}
