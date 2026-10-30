using Movies.Application.Models;

namespace Movies.Application.Abstractions;

public interface ITmdbMovieService
{
    Task<MovieList> GetMoviesAsync(
        int page,
        string language,
        CancellationToken cancellationToken = default);

    Task<MovieDetails> GetMovieDetailsAsync(
        int movieId,
        string language,
        CancellationToken cancellationToken = default);
}
