using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Responses;
using Movies.Application.Abstractions;
using Movies.Application.Models;

namespace Movies.Api.Controllers;

[ApiController]
[Route("api/movies")]
[Produces("application/json")]
public sealed class MoviesController(ITmdbMovieService movieService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(SuccessResponse<MovieList>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SuccessResponse<MovieList>>> GetMovies(
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery] string language = "en-US",
        CancellationToken cancellationToken = default)
    {
        var movies = await movieService.GetMoviesAsync(page, language, cancellationToken);
 
        return Ok(SuccessResponse<MovieList>.Create(movies));
    }
 
    [HttpGet("{movieId:int}")]
    [ProducesResponseType(typeof(SuccessResponse<MovieDetails>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SuccessResponse<MovieDetails>>> GetMovieDetails(
        [Range(1, int.MaxValue)] int movieId,
        [FromQuery] string language = "en-US",
        CancellationToken cancellationToken = default)
    {
        var movie = await movieService.GetMovieDetailsAsync(movieId, language, cancellationToken);
 
        return Ok(SuccessResponse<MovieDetails>.Create(movie));
    }
}
