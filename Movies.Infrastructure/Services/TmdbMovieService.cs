using System.Globalization;
using Microsoft.Extensions.Options;
using Movies.Application.Abstractions;
using Movies.Application.Exceptions;
using Movies.Application.mapping;
using Movies.Application.Models;
using Movies.Infrastructure.Configuration;
using Movies.Infrastructure.Http;

namespace Movies.Infrastructure.Services;

/// <summary>
/// Application-facing TMDB service. It composes TMDB endpoint requests and maps
/// successful responses to application models.
/// </summary>
public sealed class TmdbMovieService(
    ITmdbApiClient tmdbApiClient,
    IOptions<TmdbOptions> options) : ITmdbMovieService
{
    private const string DefaultLanguage = "en-US";
 
    private string? ImageBaseUrl => options.Value.ImageBaseUrl?.Trim();
 
    public async Task<MovieList> GetMoviesAsync(
        int page, string language, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
 
        var uri = "discover/movie?include_adult=false&include_video=false&certification_country=US&certification.lte=PG-13"
                  + $"&language={Lang(language)}&page={page.ToString(CultureInfo.InvariantCulture)}"
                  + "&sort_by=popularity.desc";
 
        var response = await tmdbApiClient.GetAsync(uri, cancellationToken);
        return MovieListMapper.ToDto(TmdbResponseReader.Read<MovieList>(response), ImageBaseUrl);
    }
 
    public async Task<MovieDetails> GetMovieDetailsAsync(
        int movieId, string language, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(movieId, 1);
 
        var uri = $"movie/{movieId.ToString(CultureInfo.InvariantCulture)}?language={Lang(language)}";
 
        var response = await tmdbApiClient.GetAsync(uri, cancellationToken);
        return MovieDetailsMapper.ToDto(TmdbResponseReader.Read<MovieDetails>(response), ImageBaseUrl);
    }
 
    private static string Lang(string language) =>
        Uri.EscapeDataString(string.IsNullOrWhiteSpace(language) ? DefaultLanguage : language.Trim());
}