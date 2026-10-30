using System.Text.Json;
using Movies.Application.Exceptions;
using Movies.Application.Models;
using Movies.Infrastructure.Http;

namespace Movies.Infrastructure.Services;

/// <summary>
/// Turns a raw provider response into a typed result, or a <see cref="TmdbApiException"/>.
/// Contains no HTTP-layer or controller concerns.
/// </summary>
internal static class TmdbResponseReader
{
    private const string FallbackMessage = "The movie provider returned an error.";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static T Read<T>(ProviderResponse response)
    {
        if (!IsSuccess(response.StatusCode))
            throw ToException(response);

        if (string.IsNullOrWhiteSpace(response.Body))
            throw new InvalidOperationException("The movie provider returned an empty response.");

        try
        {
            return JsonSerializer.Deserialize<T>(response.Body, JsonOptions)
                   ?? throw new InvalidOperationException("The movie provider returned an invalid response.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("The movie provider returned malformed JSON.", ex);
        }
    }

    private static bool IsSuccess(int statusCode) => statusCode is >= 200 and < 300;

    private static TmdbApiException ToException(ProviderResponse response)
    {
        var (message, code, raw) = ParseError(response.Body);

        return new TmdbApiException(
            response.StatusCode,
            message ?? FallbackMessage,
            code,
            raw,
            response.Headers);
    }

    private static (string? Message, string? Code, JsonElement? Raw) ParseError(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return (null, null, null);

        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;
            var raw = root.Clone();

            if (root.ValueKind != JsonValueKind.Object)
                return (null, null, raw);

            return (
                GetString(root, "status_message") ?? GetString(root, "message"),
                GetScalar(root, "status_code") ?? GetString(root, "error_code"),
                raw);
        }
        catch (JsonException)
        {
            // Non-JSON body (e.g. an HTML error page from a gateway).
            return (null, null, JsonSerializer.SerializeToElement(body));
        }
    }

    private static string? GetString(JsonElement element, string name) =>
        element.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String
            ? p.GetString()
            : null;

    private static string? GetScalar(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var p)) return null;

        return p.ValueKind switch
        {
            JsonValueKind.String => p.GetString(),
            JsonValueKind.Number => p.GetRawText(),
            _ => null
        };
    }
}