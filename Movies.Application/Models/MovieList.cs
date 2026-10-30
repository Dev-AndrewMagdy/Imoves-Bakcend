using System.Text.Json.Serialization;

namespace Movies.Application.Models;

/// <summary>
/// A page of movies as returned by the TMDB <c>discover/movie</c> endpoint.
/// </summary>
public sealed class MovieList
{
    [JsonPropertyName("results")]
    public IReadOnlyList<MovieSummary> Results { get; init; } = [];
    
    [JsonPropertyName("page")]
    public int Page { get; init; }
    
    [JsonPropertyName("total_pages")]
    public int TotalPages { get; init; }

    [JsonPropertyName("total_results")]
    public int TotalResults { get; init; }
}
