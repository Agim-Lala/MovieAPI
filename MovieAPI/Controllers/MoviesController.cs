﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Domain.Movies;
using MovieAPI.Services;


namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieService _movieService;

        public MoviesController(MovieService movieService)
        {
            _movieService = movieService;
        }
        
        [HttpGet("new")]
        public async Task<ActionResult<List<MovieDTO>>> GetNewMovies([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            if (page < 1 || pageSize < 1) return BadRequest("Page and pageSize must be greater than 0.");

            var movies = await _movieService.GetNewMoviesAsync(page, pageSize);
            return Ok(movies);
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieDTO>>> GetMovies()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null)
            {
                return NotFound(new { Message = "Movie not found." });
            }
            return Ok(movie);
        }
        
        
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateMovie(CreateMovieDTO movieDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var movie = await _movieService.AddMovieAsync(movieDTO);
                return CreatedAtAction(nameof(CreateMovie), new { id = movie.MovieId }, movie);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] CreateMovieDTO updatedMovie)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _movieService.UpdateMovieAsync(id, updatedMovie);
            if (!result) return NotFound();

            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var result = await _movieService.DeleteMovieAsync(id);
            if (!result) return NotFound(new { message = "Movie not found" });

            return NoContent();
        }
        
        [HttpGet("by-category")]
        public async Task<ActionResult<List<MovieDTO>>> GetMoviesByCategoryId([FromQuery] int categoryId)
        {
            var movies = await _movieService.GetMoviesByCategoryIdAsync(categoryId);
            return Ok(movies);
        }

        [HttpGet("by-director")]
        public async Task<ActionResult<List<MovieDTO>>> GetMoviesByDirectorId([FromQuery] int directorId)
        {
            var movies = await _movieService.GetMoviesByDirectorIdAsync(directorId);
            return Ok(movies);
        }

        [HttpGet("by-genre")]
        public async Task<ActionResult<List<MovieDTO>>> GetMoviesByGenreId([FromQuery] int genreId)
        {
            var movies = await _movieService.GetMoviesByGenreIdAsync(genreId);
            return Ok(movies);
        }

        [HttpGet("by-release-year")]
        public async Task<ActionResult<List<MovieDTO>>> GetMoviesByReleaseYear([FromQuery] int startYear, [FromQuery] int endYear)
        {
            var movies = await _movieService.GetMoviesByReleaseYearAsync(startYear, endYear);
            return Ok(movies);
        }
        
        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> GetFilteredMovies(
            [FromQuery] int? genreId,
            [FromQuery] int? startYear,
            [FromQuery] int? endYear,
            [FromQuery] int? qualityId) 
        {
            var movies = await _movieService.GetFilteredMoviesAsync(genreId, startYear, endYear, qualityId); // Pass qualityId
            return Ok(movies);
        }
    }
}
























