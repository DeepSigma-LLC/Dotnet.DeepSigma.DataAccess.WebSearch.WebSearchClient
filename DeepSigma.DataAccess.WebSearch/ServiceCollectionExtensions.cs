using Microsoft.Extensions.DependencyInjection;

namespace DeepSigma.DataAccess.WebSearch.Client;

/// <summary>
/// DI extensions for registering <see cref="WebSearchClient{TSearchOptions}"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="WebSearchClient{TSearchOptions}"/> as a singleton. Expects the three
    /// collaborators (<c>IUrlRetriever&lt;TSearchOptions&gt;</c>, <c>IHtmlRetriever</c>,
    /// <c>IContentExtractor</c>) to already be registered.
    /// </summary>
    public static IServiceCollection AddWebSearchClient<TSearchOptions>(this IServiceCollection services)
        where TSearchOptions : class
    {
        services.AddSingleton<WebSearchClient<TSearchOptions>>();
        return services;
    }
}
