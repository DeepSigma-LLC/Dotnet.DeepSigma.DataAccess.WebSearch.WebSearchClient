using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DeepSigma.DataAccess.WebSearch;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="ISearxngClient"/> with a typed <see cref="System.Net.Http.HttpClient"/>,
    /// options validation, and a standard resilience pipeline (retry + circuit breaker + timeout).
    /// </summary>
    public static IServiceCollection AddSearxngClient(
        this IServiceCollection services,
        Action<SearxngOptions> configure)
    {
        services.AddOptions<SearxngOptions>()
            .Configure(configure)
            .Validate(
                o => o.BaseUri is not null && o.BaseUri.IsAbsoluteUri,
                "BaseUri must be set to an absolute URI.")
            .Validate(
                o => o.Timeout > TimeSpan.Zero && o.Timeout <= TimeSpan.FromMinutes(5),
                "Timeout must be between 0 and 5 minutes.")
            .ValidateOnStart();

        services.AddHttpClient<ISearxngClient, SearxngClient>((sp, http) =>
        {
            var options = sp.GetRequiredService<IOptions<SearxngOptions>>().Value;

            http.BaseAddress = options.BaseUri;
            // Let the resilience pipeline own timeouts; prevent HttpClient's
            // hard timeout from racing against the pipeline's attempt timeout.
            http.Timeout = Timeout.InfiniteTimeSpan;

            if (!string.IsNullOrWhiteSpace(options.UserAgent))
                http.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);

            http.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        })
        .AddStandardResilienceHandler();

        return services;
    }
}
