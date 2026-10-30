using System.Text.Json.Serialization;
using Movies.Api.ErrorHandling;

namespace Movies.Api.Responses;

public sealed class ErrorResponse
{
    [JsonPropertyName("message")] public string Message { get; init; } = "Something went wrong";

    [JsonPropertyName("errorMessage")] public string ErrorMessage { get; init; } = string.Empty;

    [JsonPropertyName("errorKey")] public string ErrorKey { get; init; } = ErrorKeys.InternalServerError;

    [JsonPropertyName("statusCode")] public int StatusCode { get; init; }

    [JsonPropertyName("stacktrace")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Stacktrace { get; init; }
}