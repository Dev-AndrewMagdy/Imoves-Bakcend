using Microsoft.AspNetCore.Diagnostics;
using Movies.Application.Exceptions;
using Movies.Api.Responses;

namespace Movies.Api.ErrorHandling;

/// <summary>
/// Maps every unhandled exception to <see cref="ErrorResponse"/>.
/// Internal details (exception message, stacktrace) are exposed in Development only.
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Client disconnected: no one to respond to.
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            httpContext.Response.StatusCode = 499;
            return true;
        }

        var (status, key, message) = Map(exception);

        if (status >= 500)
            logger.LogError(exception, "Request failed: {Status} {Key}", status, key);
        else
            logger.LogWarning(exception, "Request rejected: {Status} {Key}", status, key);

        if (exception is TmdbApiException tmdbException)
        {
            foreach (var (name, values) in tmdbException.Headers)
            {
                httpContext.Response.Headers[name] = values;
            }
        }

        var isDev = environment.IsDevelopment();

        await ErrorResponseWriter.WriteAsync(
            httpContext,
            status,
            key,
            message,
            errorMessage: isDev ? exception.Message : null,
            stacktrace:   isDev ? exception.ToString() : null,
            cancellationToken);

        return true;
    }

    private static (int Status, string Key, string Message) Map(Exception exception) => exception switch
    {
        ArgumentOutOfRangeException =>
            (StatusCodes.Status400BadRequest, ErrorKeys.InvalidRequest, "Invalid request parameter."),

        TmdbApiException { StatusCode: 404 } =>
            (StatusCodes.Status404NotFound, ErrorKeys.ProviderNotFound, "The requested movie was not found."),

        TmdbApiException { StatusCode: 429 } =>
            (StatusCodes.Status503ServiceUnavailable, ErrorKeys.ProviderBusy, "The service is busy. Please try again shortly."),

        TmdbApiException =>
            (StatusCodes.Status502BadGateway, ErrorKeys.ProviderError, "The movie service failed to process the request."),

        HttpRequestException =>
            (StatusCodes.Status502BadGateway, ErrorKeys.ProviderUnreachable, "The movie service is unreachable."),

        TaskCanceledException =>
            (StatusCodes.Status504GatewayTimeout, ErrorKeys.ProviderTimeout, "The movie service timed out."),

        _ =>
            (StatusCodes.Status500InternalServerError, ErrorKeys.InternalServerError, "Something went wrong")
    };
}