using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Movies.Application.Abstractions;
using Movies.Infrastructure.Configuration;
using Movies.Infrastructure.Http;
using Movies.Infrastructure.Services;

namespace Movies.Infrastructure;

public static class di
{
    public static IServiceCollection AddTmdbIntegration(this IServiceCollection services)
    {
        // Bind once, validate at startup (host fails fast with OptionsValidationException).
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IValidateOptions<TmdbOptions>, TmdbOptionsValidator>());
 
        services.AddOptions<TmdbOptions>()
            .BindConfiguration(TmdbOptions.SectionName)
            .ValidateOnStart();
 
        services.AddHttpClient<ITmdbApiClient, TmdbApiClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<TmdbOptions>>().Value;
 
                client.BaseAddress = new Uri($"{options.BaseUrl.TrimEnd('/')}/", UriKind.Absolute);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
 
                // Remove if TmdbApiClient already sets the Authorization header itself.
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", options.ReadAccessToken);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                // Return upstream redirects and errors to the caller rather than following them.
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.All
            });
 
        services.AddTransient<ITmdbMovieService, TmdbMovieService>();
        return services;
    }
}
