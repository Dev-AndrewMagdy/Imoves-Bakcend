using System.Net;
using Movies.Api.Tests.Infrastructure;

namespace Movies.Api.Tests;

public sealed class SwaggerTests
{
    [Fact]
    public async Task SwaggerDocument_IsAvailableInDevelopmentAndIncludesMoviesEndpoints()
    {
        var handler = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        await using var factory = new TmdbApiFactory(handler, "Development");
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"title\": \"Movies API\"", document);
        Assert.Contains("/api/movies", document);
        Assert.Contains("/api/movies/{movieId}", document);
        Assert.Contains("\"MovieList\"", document);
        Assert.Contains("\"MovieDetails\"", document);
    }

    [Fact]
    public async Task SwaggerUi_IsAvailableInDevelopment()
    {
        var handler = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        await using var factory = new TmdbApiFactory(handler, "Development");
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains(
            "swagger-ui",
            await response.Content.ReadAsStringAsync(),
            StringComparison.OrdinalIgnoreCase);
    }
}
