using System.Text.Json.Serialization;

namespace Movies.Application.Models;

public sealed class Genre
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
