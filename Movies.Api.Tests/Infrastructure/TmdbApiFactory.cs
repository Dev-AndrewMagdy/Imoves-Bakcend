using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Movies.Infrastructure.Http;

namespace Movies.Api.Tests.Infrastructure;

internal sealed class TmdbApiFactory(
    StubHttpMessageHandler handler,
    string environment = "Testing") : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment);

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Tmdb:BaseUrl"] = "https://api.themoviedb.org/3/",
                ["Tmdb:ImageBaseUrl"] = "https://image.tmdb.org/t/p/",
                ["Tmdb:ReadAccessToken"] = "test-read-token",
                ["Tmdb:TimeoutSeconds"] = "5"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ITmdbApiClient>();
            services.AddHttpClient<ITmdbApiClient, TmdbApiClient>()
                .ConfigurePrimaryHttpMessageHandler(() => handler);
        });
    }
}
