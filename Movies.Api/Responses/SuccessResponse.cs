using System.Text.Json.Serialization;

namespace Movies.Api.Responses;

public sealed class SuccessResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success => true;

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("data")]
    public T? Data { get; init; }

    public static SuccessResponse<T> Create(
        T? data,
        string? message = null) => new()
    {
        Data = data,
        Message = message
    };
}