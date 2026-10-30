using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Movies.Api.Responses;
using Movies.Api.Tests.Infrastructure;
using Movies.Application.Models;

namespace Movies.Api.Tests;

public sealed class MoviesEndpointsTests
{
    [Fact]
    public async Task GetMovies_ForwardsPagingLanguageDefaultsAndBearerToken()
    {
        const string providerJson = """{"page":2,"results":[{"id":969681,"title":"Lilo & Stitch"}]}""";
        var handler = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse(providerJson)));
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies?page=2&language=es-ES");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var data = await ReadDataAsync<MovieList>(response);
        Assert.Equal(2, data.Page);
        var movie = Assert.Single(data.Results);
        Assert.Equal(969681, movie.Id);
        Assert.Equal("Lilo & Stitch", movie.Title);
        Assert.Equal("https://api.themoviedb.org/3/discover/movie", handler.LastRequestUri!.GetLeftPart(UriPartial.Path));
        Assert.Contains("include_adult=false", handler.LastRequestUri.Query);
        Assert.Contains("include_video=false", handler.LastRequestUri.Query);
        Assert.Contains("language=es-ES", handler.LastRequestUri.Query);
        Assert.Contains("page=2", handler.LastRequestUri.Query);
        Assert.Contains("sort_by=popularity.desc", handler.LastRequestUri.Query);
        Assert.Equal("Bearer test-read-token", handler.LastAuthorization);
    }

    [Fact]
    public async Task GetMovies_WhenLanguageIsOmitted_UsesEnglishDefault()
    {
        var handler = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse("""{"page":1}""")));
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("language=en-US", handler.LastRequestUri!.Query);
        Assert.Contains("page=1", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task GetMovies_MapsTheFullProviderPayloadAndImageUrls()
    {
        const string providerJson = """{"page":1,"results":[{"adult":false,"backdrop_path":"/r08LxMJCbf1SiJIWeyH9aJmdqwC.jpg","genre_ids":[878,28,12],"id":969681,"title":"Spider-Man: Brand New Day","original_language":"en","original_title":"Spider-Man: Brand New Day","overview":"Fighting crime full-time as Spider-Man in a world that doesn't remember him.","popularity":868.7427,"poster_path":"/bjiS5ipwxb9JFy3XRRN4OAilSeX.jpg","release_date":"2026-07-29","softcore":false,"video":false,"vote_average":8.474,"vote_count":6177}],"total_pages":1001,"total_results":20001}""";
        var handler = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse(providerJson)));
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var data = await ReadDataAsync<MovieList>(response);

        Assert.Equal(1, data.Page);
        Assert.Equal(1001, data.TotalPages);
        Assert.Equal(20001, data.TotalResults);
        var movie = Assert.Single(data.Results);
        Assert.False(movie.Adult);
        Assert.Equal(
            "https://image.tmdb.org/t/p/w1280/r08LxMJCbf1SiJIWeyH9aJmdqwC.jpg",
            movie.BackdropPath);
        Assert.Equal(new[] { 878, 28, 12 }, movie.GenreIds);
        Assert.Equal(969681, movie.Id);
        Assert.Equal("Spider-Man: Brand New Day", movie.Title);
        Assert.Equal("en", movie.OriginalLanguage);
        Assert.Equal("Spider-Man: Brand New Day", movie.OriginalTitle);
        Assert.Equal(868.7427, movie.Popularity);
        Assert.Equal(
            "https://image.tmdb.org/t/p/w500/bjiS5ipwxb9JFy3XRRN4OAilSeX.jpg",
            movie.PosterPath);
        Assert.Equal("2026-07-29", movie.ReleaseDate);
        Assert.False(movie.Softcore);
        Assert.False(movie.Video);
        Assert.Equal(8.474, movie.VoteAverage);
        Assert.Equal(6177, movie.VoteCount);
    }

    [Fact]
    public async Task GetMovieDetails_ForwardsMovieIdAndLanguage()
    {
        const string providerJson = """{"id":969681,"title":"Lilo & Stitch","overview":"A story."}""";
        var handler = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse(providerJson)));
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies/969681?language=fr-FR");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var data = await ReadDataAsync<MovieDetails>(response);
        Assert.Equal(969681, data.Id);
        Assert.Equal("Lilo & Stitch", data.Title);
        Assert.Equal("A story.", data.Overview);
        Assert.Equal("https://api.themoviedb.org/3/movie/969681", handler.LastRequestUri!.GetLeftPart(UriPartial.Path));
        Assert.Equal("?language=fr-FR", handler.LastRequestUri.Query);
        Assert.Equal("Bearer test-read-token", handler.LastAuthorization);
    }

    [Fact]
    public async Task GetMovieDetails_MapsTheFullProviderPayload()
    {
        const string providerJson = """{"adult":false,"backdrop_path":"/4tjxYg2AbOzzG9G1YuiT2NzYhlH.jpg","belongs_to_collection":{"id":531241,"name":"Spider-Man (MCU) Collection","poster_path":"/yeba3Y820OjApZO9aJevDsNeMEd.jpg","backdrop_path":"/AvnqpRwlEaYNVL6wzC4RN94EdSd.jpg"},"budget":225000000,"genres":[{"id":878,"name":"Science Fiction"},{"id":28,"name":"Action"},{"id":12,"name":"Adventure"}],"homepage":"https://spidermanbrandnewday.movie","id":969681,"imdb_id":"tt22084616","origin_country":["US"],"original_language":"en","original_title":"Spider-Man: Brand New Day","overview":"Fighting crime full-time as Spider-Man.","popularity":421.3022,"poster_path":"/bjiS5ipwxb9JFy3XRRN4OAilSeX.jpg","production_companies":[{"id":420,"logo_path":"/hUzeosd33nzE5MCNsZxCGEKTXaQ.png","name":"Marvel Studios","origin_country":"US"},{"id":5,"logo_path":"/71BqEFAF4V3qjjMPCpLuyJFB9A.png","name":"Columbia Pictures","origin_country":"US"}],"production_countries":[{"iso_3166_1":"US","name":"United States of America"}],"release_date":"2026-07-29","revenue":2517281128,"runtime":145,"softcore":false,"spoken_languages":[{"english_name":"English","iso_639_1":"en","name":"English"}],"status":"Released","tagline":"A brand new day starts now.","title":"Spider-Man: Brand New Day","video":false,"vote_average":8.476,"vote_count":6323}""";
        var handler = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse(providerJson)));
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies/969681");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var data = await ReadDataAsync<MovieDetails>(response);

        Assert.False(data.Adult);
        Assert.Equal("https://image.tmdb.org/t/p/w1280/4tjxYg2AbOzzG9G1YuiT2NzYhlH.jpg", data.BackdropPath);
        Assert.Equal("https://image.tmdb.org/t/p/w500/bjiS5ipwxb9JFy3XRRN4OAilSeX.jpg", data.PosterPath);
        Assert.NotNull(data.BelongsToCollection);
        Assert.Equal(531241, data.BelongsToCollection.Id);
        Assert.Equal("Spider-Man (MCU) Collection", data.BelongsToCollection.Name);
        Assert.Equal(
            "https://image.tmdb.org/t/p/w500/yeba3Y820OjApZO9aJevDsNeMEd.jpg",
            data.BelongsToCollection.PosterPath);
        Assert.Equal(
            "https://image.tmdb.org/t/p/w1280/AvnqpRwlEaYNVL6wzC4RN94EdSd.jpg",
            data.BelongsToCollection.BackdropPath);
        Assert.Equal(225000000, data.Budget);
        Assert.Collection(
            data.Genres,
            genre => Assert.Equal("Science Fiction", genre.Name),
            genre => Assert.Equal("Action", genre.Name),
            genre => Assert.Equal("Adventure", genre.Name));
        Assert.Equal("https://spidermanbrandnewday.movie", data.Homepage);
        Assert.Equal("tt22084616", data.ImdbId);
        Assert.Equal(new[] { "US" }, data.OriginCountry);
        Assert.Equal("Spider-Man: Brand New Day", data.OriginalTitle);
        Assert.Equal(421.3022, data.Popularity);
        Assert.Equal(2, data.ProductionCompanies.Count);
        Assert.Equal("Marvel Studios", data.ProductionCompanies[0].Name);
        Assert.Equal(
            "https://image.tmdb.org/t/p/w300/hUzeosd33nzE5MCNsZxCGEKTXaQ.png",
            data.ProductionCompanies[0].LogoPath);
        Assert.Equal("US", data.ProductionCountries[0].Iso31661);
        Assert.Equal("2026-07-29", data.ReleaseDate);
        Assert.Equal(2517281128L, data.Revenue);
        Assert.Equal(145, data.Runtime);
        Assert.False(data.Softcore);
        Assert.Equal("English", data.SpokenLanguages[0].EnglishName);
        Assert.Equal("Released", data.Status);
        Assert.Equal("A brand new day starts now.", data.Tagline);
        Assert.Equal(8.476, data.VoteAverage);
        Assert.Equal(6323, data.VoteCount);
    }

    [Fact]
    public async Task GetMovieDetails_WithNonIntegerId_DoesNotMatchDetailsRoute()
    {
        var handler = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse("{}")));
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies/not-a-number");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Null(handler.LastRequestUri);
    }

    [Fact]
    public async Task GetMovieDetails_WhenProviderReturnsNotFound_MapsToNotFoundError()
    {
        const string providerJson = """{"success":false,"status_code":34,"status_message":"The resource you requested could not be found."}""";
        var handler = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(JsonResponse(providerJson, HttpStatusCode.NotFound)));
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertErrorResponse(response, "movie_not_found");
    }

    [Fact]
    public async Task GetMovieDetails_WhenProviderIsRateLimited_ReturnsServiceUnavailableAndForwardsRetryAfter()
    {
        const string providerJson = """{"success":false,"status_code":25,"status_message":"Your request count is over the allowed limit."}""";
        var handler = new StubHttpMessageHandler((_, _) =>
        {
            var response = JsonResponse(providerJson, HttpStatusCode.TooManyRequests);
            response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(
                TimeSpan.FromSeconds(30));
            return Task.FromResult(response);
        });
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies/999999");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        await AssertErrorResponse(response, "provider_busy");
        Assert.Equal(30, response.Headers.RetryAfter?.Delta?.TotalSeconds);
    }

    [Fact]
    public async Task GetMovies_WhenUpstreamConnectionFails_ReturnsBadGateway()
    {
        var handler = new StubHttpMessageHandler((_, _) =>
            Task.FromException<HttpResponseMessage>(new HttpRequestException("connection refused")));
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies");

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        await AssertErrorResponse(response, "provider_unreachable");
    }

    [Fact]
    public async Task GetMovies_WhenUpstreamTimesOut_ReturnsGatewayTimeout()
    {
        var handler = new StubHttpMessageHandler((_, _) =>
            Task.FromException<HttpResponseMessage>(new TaskCanceledException("upstream timeout")));
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies");

        Assert.Equal(HttpStatusCode.GatewayTimeout, response.StatusCode);
        await AssertErrorResponse(response, "provider_timeout");
    }

    [Fact]
    public async Task GetMovies_WhenUnexpectedExceptionOccurs_ReturnsInternalServerError()
    {
        var handler = new StubHttpMessageHandler((_, _) =>
            Task.FromException<HttpResponseMessage>(new InvalidOperationException("boom")));
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        await AssertErrorResponse(response, "internal_server_error");
    }

    [Fact]
    public async Task GetMovies_WithInvalidPage_ReturnsBadRequestWithoutCallingUpstream()
    {
        var handler = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse("{}")));
        await using var factory = new TmdbApiFactory(handler);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movies?page=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Null(handler.LastRequestUri);
        await AssertErrorResponse(response, "validation_failed");
    }

    private static HttpResponseMessage JsonResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    private static async Task<T> ReadDataAsync<T>(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<SuccessResponse<T>>();

        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.Null(payload.Message);
        Assert.NotNull(payload.Data);

        return payload.Data;
    }

    private static async Task AssertErrorResponse(
        HttpResponseMessage response,
        string expectedErrorKey)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        Assert.Equal(expectedErrorKey, root.GetProperty("errorKey").GetString());
        Assert.Equal((int)response.StatusCode, root.GetProperty("statusCode").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("message").GetString()));
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }
}
