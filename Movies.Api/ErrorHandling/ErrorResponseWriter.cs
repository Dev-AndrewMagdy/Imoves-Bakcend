using Microsoft.AspNetCore.Http;

namespace Movies.Api.Responses;

/// <summary>The only place that serializes <see cref="ErrorResponse"/> to the wire.</summary>
public static class ErrorResponseWriter
{
    public static Task WriteAsync(
        HttpContext context,
        int statusCode,
        string errorKey,
        string message,
        string? errorMessage = null,
        string? stacktrace = null,
        CancellationToken cancellationToken = default)
    {
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode   = statusCode,
            ErrorKey     = errorKey,
            Message      = message,
            ErrorMessage = errorMessage ?? string.Empty,
            Stacktrace   = stacktrace
        }, cancellationToken);
    }
}