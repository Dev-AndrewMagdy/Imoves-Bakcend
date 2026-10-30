using System.Text.Json.Serialization;

namespace Movies.Application.Models;

public sealed class ProductionCompany
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("logo_path")]
    public string? LogoPath { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("origin_country")]
    public string? OriginCountry { get; init; }
}
