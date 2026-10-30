using Movies.Application.Models;

namespace Movies.Application.mapping;

public static class MovieDetailsMapper
{
    public static MovieDetails ToDto(
        MovieDetails movie,
        string? imageBaseUrl)
    {
        return new MovieDetails
        {
            Adult = movie.Adult,
            BackdropPath = ImageUrlBuilder.Build(movie.BackdropPath, imageBaseUrl, "w1280"),
            BelongsToCollection = movie.BelongsToCollection is null
                ? null
                : new MovieCollection
                {
                    Id = movie.BelongsToCollection.Id,
                    Name = movie.BelongsToCollection.Name,
                    PosterPath = ImageUrlBuilder.Build(
                        movie.BelongsToCollection.PosterPath, imageBaseUrl, "w500"),
                    BackdropPath = ImageUrlBuilder.Build(
                        movie.BelongsToCollection.BackdropPath, imageBaseUrl, "w1280")
                },
            Budget = movie.Budget,
            Genres = movie.Genres
                .Select(genre => new Genre
                {
                    Id = genre.Id,
                    Name = genre.Name
                })
                .ToList(),
            Homepage = movie.Homepage,
            Id = movie.Id,
            ImdbId = movie.ImdbId,
            OriginCountry = movie.OriginCountry.ToList(),
            OriginalLanguage = movie.OriginalLanguage,
            OriginalTitle = movie.OriginalTitle,
            Overview = movie.Overview,
            Popularity = movie.Popularity,
            PosterPath = ImageUrlBuilder.Build(movie.PosterPath, imageBaseUrl, "w500"),
            ProductionCompanies = movie.ProductionCompanies
                .Select(company => new ProductionCompany
                {
                    Id = company.Id,
                    Name = company.Name,
                    OriginCountry = company.OriginCountry,
                    LogoPath = ImageUrlBuilder.Build(company.LogoPath, imageBaseUrl, "w300")
                })
                .ToList(),
            ProductionCountries = movie.ProductionCountries
                .Select(country => new ProductionCountry
                {
                    Iso31661 = country.Iso31661,
                    Name = country.Name
                })
                .ToList(),
            ReleaseDate = movie.ReleaseDate,
            Revenue = movie.Revenue,
            Runtime = movie.Runtime,
            Softcore = movie.Softcore,
            SpokenLanguages = movie.SpokenLanguages
                .Select(language => new SpokenLanguage
                {
                    EnglishName = language.EnglishName,
                    Iso6391 = language.Iso6391,
                    Name = language.Name
                })
                .ToList(),
            Status = movie.Status,
            Tagline = movie.Tagline,
            Title = movie.Title,
            Video = movie.Video,
            VoteAverage = movie.VoteAverage,
            VoteCount = movie.VoteCount
        };
    }
}
