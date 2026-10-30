using Microsoft.Extensions.Options;

namespace Movies.Infrastructure.Configuration;

public sealed class TmdbOptions
{
    public const string SectionName = "Tmdb";

    public string BaseUrl { get; init; } = string.Empty;

    public string ImageBaseUrl { get; init; } = string.Empty;

    public string ReadAccessToken { get; init; } = string.Empty;

    public int TimeoutSeconds { get; init; } = 15;
}
internal sealed class TmdbOptionsValidator : IValidateOptions<TmdbOptions>
{
    public ValidateOptionsResult Validate(string? name, TmdbOptions options)
    {
        var errors = new List<string>();
 
        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri)
            || baseUri.Scheme != Uri.UriSchemeHttps)
            errors.Add("Tmdb:BaseUrl must be an absolute HTTPS URL.");
 
        if (string.IsNullOrWhiteSpace(options.ReadAccessToken))
            errors.Add("Tmdb:ReadAccessToken is not configured. Use user secrets or the Tmdb__ReadAccessToken environment variable.");
 
        if (options.TimeoutSeconds <= 0)
            errors.Add("Tmdb:TimeoutSeconds must be greater than zero.");
 
        return errors.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(errors);
    }
}