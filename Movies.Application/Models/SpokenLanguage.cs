using System.Text.Json.Serialization;

namespace Movies.Application.Models;

public sealed class SpokenLanguage
{
    [JsonPropertyName("english_name")]
    public string? EnglishName { get; init; }

    [JsonPropertyName("iso_639_1")]
    public string? Iso6391 { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
