using System.Text.Json.Serialization;

namespace Movies.Application.Models;

public sealed class ProductionCountry
{
    [JsonPropertyName("iso_3166_1")]
    public string? Iso31661 { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
