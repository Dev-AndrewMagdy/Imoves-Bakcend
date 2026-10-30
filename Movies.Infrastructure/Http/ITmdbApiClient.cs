using Movies.Application.Models;

namespace Movies.Infrastructure.Http;

public interface ITmdbApiClient
{
    Task<ProviderResponse> GetAsync(string relativeUri, CancellationToken cancellationToken = default);
}
