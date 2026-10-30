namespace Movies.Api.ErrorHandling;

public static class ErrorKeys
{
    public const string InternalServerError = "internal_server_error";
    public const string InvalidRequest      = "invalid_request";
    public const string ValidationFailed    = "validation_failed";
    public const string NotFound            = "not_found";
    public const string MethodNotAllowed    = "method_not_allowed";
    public const string Unauthorized        = "unauthorized";
    public const string Forbidden           = "forbidden";
    public const string ProviderNotFound    = "movie_not_found";
    public const string ProviderBusy        = "provider_busy";
    public const string ProviderError       = "provider_error";
    public const string ProviderUnreachable = "provider_unreachable";
    public const string ProviderTimeout     = "provider_timeout";
}
