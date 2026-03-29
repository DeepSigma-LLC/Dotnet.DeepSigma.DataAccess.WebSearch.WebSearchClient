namespace DeepSigma.DataAccess.WebSearch.Exceptions;

/// <summary>The request exceeded the configured timeout.</summary>
public sealed class SearxngTimeoutException : SearxngException
{
    public SearxngTimeoutException(string message) : base(message) { }

    public SearxngTimeoutException(string message, Exception innerException)
        : base(message, innerException) { }
}
