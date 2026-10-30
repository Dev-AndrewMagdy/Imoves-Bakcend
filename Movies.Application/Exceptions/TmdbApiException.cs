using System.Text.Json;

namespace Movies.Application.Exceptions;

/// <summary>
/// Raised when TMDB returns a non-success status code. The original status,
/// error code, body, and headers are kept so they can be returned to the client.
/// </summary>
public sealed class TmdbApiException(
    int statusCode,
    string message,
    string? errorCode,
    JsonElement? providerError,
    IReadOnlyDictionary<string, string[]> headers) : Exception(message)
{
    public int StatusCode { get; } = statusCode;

    public string? ErrorCode { get; } = errorCode;

    public JsonElement? ProviderError { get; } = providerError;

    public IReadOnlyDictionary<string, string[]> Headers { get; } = headers;
}
