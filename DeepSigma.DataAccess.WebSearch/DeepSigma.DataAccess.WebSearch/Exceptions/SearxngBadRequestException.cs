namespace DeepSigma.DataAccess.WebSearch.Exceptions;

/// <summary>The instance returned a 4xx status other than 403.</summary>
public sealed class SearxngBadRequestException : SearxngException
{
    public int StatusCode { get; }

    public SearxngBadRequestException(string message, int statusCode) : base(message)
        => StatusCode = statusCode;

    public SearxngBadRequestException(string message, int statusCode, Exception innerException)
        : base(message, innerException) => StatusCode = statusCode;
}
