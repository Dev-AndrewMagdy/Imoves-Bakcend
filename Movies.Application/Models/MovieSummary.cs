using System.Text.Json.Serialization;

namespace Movies.Application.Models;

/// <summary>
/// A movie entry inside a TMDB list response.
/// </summary>
public sealed class MovieSummary
{
    [JsonPropertyName("adult")]
    public bool Adult { get; init; }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; init; }

    [JsonPropertyName("genre_ids")]
    public IReadOnlyList<int> GenreIds { get; init; } = [];

    [JsonPropertyName("id")]
    public int Id { get; init; }

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

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; init; }

    [JsonPropertyName("softcore")]
    public bool Softcore { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("video")]
    public bool Video { get; init; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; init; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; init; }
}
