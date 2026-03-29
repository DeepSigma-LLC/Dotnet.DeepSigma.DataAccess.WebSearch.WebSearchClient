namespace DeepSigma.DataAccess.WebSearch.Exceptions;

/// <summary>The response body could not be parsed into the expected shape.</summary>
public sealed class SearxngParseException : SearxngException
{
    public SearxngParseException(string message) : base(message) { }

    public SearxngParseException(string message, Exception innerException)
        : base(message, innerException) { }
}
