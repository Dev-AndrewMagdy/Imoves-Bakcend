namespace Movies.Application.Models;

/// <summary>
/// An unmodified response received from an upstream provider.
/// The body is kept as JSON text so the service layer can map it to application models.
/// </summary>
public sealed record ProviderResponse(
    int StatusCode,
    string? ContentType,
    string Body,
    IReadOnlyDictionary<string, string[]> Headers);
