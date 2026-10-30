using System.Text.Json.Serialization;

namespace Movies.Application.Models;

/// <summary>
/// Movie details as returned by the TMDB <c>movie/{movieId}</c> endpoint.
/// </summary>
public sealed class MovieDetails
{
    [JsonPropertyName("adult")]
    public bool Adult { get; init; }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; init; }

    [JsonPropertyName("belongs_to_collection")]
    public MovieCollection? BelongsToCollection { get; init; }

    [JsonPropertyName("budget")]
    public long Budget { get; init; }

    [JsonPropertyName("genres")]
    public IReadOnlyList<Genre> Genres { get; init; } = [];

    [JsonPropertyName("homepage")]
    public string? Homepage { get; init; }

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("imdb_id")]
    public string? ImdbId { get; init; }

    [JsonPropertyName("origin_country")]
    public IReadOnlyList<string> OriginCountry { get; init; } = [];

    [JsonPropertyName("original_language")]
    public string? OriginalLanguage { get; init; }

    [JsonPropertyName("original_title")]
    public string? OriginalTitle { get; init; }

    [JsonPropertyName("overview")]
    public string? Overview { get; init; }

    [JsonPropertyName("popularity")]
    public double Popularity { get; init; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; init; }

    [JsonPropertyName("production_companies")]
    public IReadOnlyList<ProductionCompany> ProductionCompanies { get; init; } = [];

    [JsonPropertyName("production_countries")]
    public IReadOnlyList<ProductionCountry> ProductionCountries { get; init; } = [];

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; init; }

    [JsonPropertyName("revenue")]
    public long Revenue { get; init; }

    [JsonPropertyName("runtime")]
    public int? Runtime { get; init; }

    [JsonPropertyName("softcore")]
    public bool Softcore { get; init; }

    [JsonPropertyName("spoken_languages")]
    public IReadOnlyList<SpokenLanguage> SpokenLanguages { get; init; } = [];

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("tagline")]
    public string? Tagline { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("video")]
    public bool Video { get; init; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; init; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; init; }
}
