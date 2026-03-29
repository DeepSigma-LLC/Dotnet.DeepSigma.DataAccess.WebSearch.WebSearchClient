namespace DeepSigma.DataAccess.WebSearch.Exceptions;

public abstract class SearxngException : Exception
{
    protected SearxngException(string message) : base(message) { }

    protected SearxngException(string message, Exception innerException)
        : base(message, innerException) { }
}
