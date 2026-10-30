
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Movies.Api.ErrorHandling;
using Movies.Api.Responses;
using Movies.Infrastructure;
using Movies.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services
    .AddOptions<TmdbOptions>()
    .BindConfiguration(TmdbOptions.SectionName)
    .Validate(
        options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri)
                   && baseUri.Scheme == Uri.UriSchemeHttps,
        "Invalid TMDB BaseUrl.")
    .Validate(
        options => Uri.TryCreate(options.ImageBaseUrl, UriKind.Absolute, out _),
        "Invalid TMDB ImageBaseUrl.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.ReadAccessToken),
        "TMDB access token is required.")
    .Validate(
        options => options.TimeoutSeconds > 0,
        "Invalid TMDB TimeoutSeconds.")
    .ValidateOnStart();

// Application services
builder.Services.AddTmdbIntegration();

// Error handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Controllers and validation
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var details = string.Join("; ", context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .SelectMany(entry => entry.Value!.Errors.Select(error =>
                    string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? $"{entry.Key} is invalid"
                        : error.ErrorMessage)));

            return new BadRequestObjectResult(new ErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorKey = ErrorKeys.ValidationFailed,
                Message = "One or more validation errors occurred.",
                ErrorMessage = details
            });
        };
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Movies API",
        Version = "v1",
        Description = "Backend API for TMDB movie listings and details."
    });
});

var app = builder.Build();

// Exception handling must be early in the pipeline.
app.UseExceptionHandler(_ => { });

// Status codes without response bodies.
app.UseStatusCodePages(context =>
{
    var status = context.HttpContext.Response.StatusCode;

    var (key, message) = status switch
    {
        StatusCodes.Status400BadRequest =>
            (ErrorKeys.InvalidRequest, "Bad request."),

        StatusCodes.Status401Unauthorized =>
            (ErrorKeys.Unauthorized, "Authentication is required."),

        StatusCodes.Status403Forbidden =>
            (ErrorKeys.Forbidden, "You do not have access to this resource."),

        StatusCodes.Status404NotFound =>
            (ErrorKeys.NotFound, "The requested resource was not found."),

        StatusCodes.Status405MethodNotAllowed =>
            (ErrorKeys.MethodNotAllowed, "Method not allowed."),

        _ =>
            (status >= 500
                ? ErrorKeys.InternalServerError
                : ErrorKeys.InvalidRequest,
             "Something went wrong.")
    };

    return ErrorResponseWriter.WriteAsync(
        context.HttpContext,
        status,
        key,
        message);
});

// Development tools

app.UseSwagger();
app.UseSwaggerUI(options =>
{ 
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Movies API v1");
    options.DocumentTitle = "Movies API";
    options.DisplayRequestDuration(); 
});


app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program;
