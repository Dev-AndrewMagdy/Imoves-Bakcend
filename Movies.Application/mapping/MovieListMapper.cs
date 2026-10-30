using Movies.Application.Models;

namespace Movies.Application.mapping;

public static class MovieListMapper
{
    public static MovieList ToDto(
        MovieList movieList,
        string? imageBaseUrl)
    {
        return new MovieList
        {
            Page = movieList.Page,
            Results = movieList.Results
                .Select(movie => new MovieSummary
                {
                    Adult = movie.Adult,
                    BackdropPath = ImageUrlBuilder.Build(movie.BackdropPath, imageBaseUrl, "w1280"),
                    GenreIds = movie.GenreIds.ToList(),
                    Id = movie.Id,
                    OriginalLanguage = movie.OriginalLanguage,
                    OriginalTitle = movie.OriginalTitle,
                    Overview = movie.Overview,
                    Popularity = movie.Popularity,
                    PosterPath = ImageUrlBuilder.Build(movie.PosterPath, imageBaseUrl, "w500"),
                    ReleaseDate = movie.ReleaseDate,
                    Softcore = movie.Softcore,
                    Title = movie.Title,
                    Video = movie.Video,
                    VoteAverage = movie.VoteAverage,
                    VoteCount = movie.VoteCount
                })
                .ToList(),
            TotalPages = movieList.TotalPages,
            TotalResults = movieList.TotalResults
        };
    }
}
