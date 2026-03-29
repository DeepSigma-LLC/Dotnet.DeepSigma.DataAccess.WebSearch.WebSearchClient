namespace DeepSigma.DataAccess.WebSearch.Exceptions;

/// <summary>Network-level failures: DNS, connection refused, etc.</summary>
public sealed class SearxngUnavailableException : SearxngException
{
    public SearxngUnavailableException(string message) : base(message) { }

    public SearxngUnavailableException(string message, Exception innerException)
        : base(message, innerException) { }
}
