using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Domain.Movies;
using MovieAPI.Enums;
using MovieAPI.Services;


namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieService _movieService;
        private readonly FileUploadHelper _fileUploadHelper;

        public MoviesController(MovieService movieService)
        {
            _movieService = movieService;
        }


        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<MovieDTO>> Create([FromForm] CreateMovieDTO dto)
        {
            var createdMovieDto = await _movieService.CreateMovieAsync(dto);
            return CreatedAtAction(nameof(GetMovieById), new { id = createdMovieDto.MovieId }, createdMovieDto);
        }


        
        [HttpGet("new")]
        public async Task<ActionResult<List<MovieDTO>>> GetNewMovies([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            if (page < 1 || pageSize < 1) return BadRequest("Page and pageSize must be greater than 0.");

            var movies = await _movieService.GetNewMoviesAsync(page, pageSize);
            return Ok(movies);
        }

        [HttpGet("sorted")]
        public async Task<IActionResult> GetSortedMoviesAsync([FromQuery] string sortBy, int page, int pageSize,
            [FromQuery] bool ascending = true)
        {
            if (!Enum.TryParse<MovieSortOption>(sortBy, true, out var sortOption))
            {
                return BadRequest($"Invalid sort option: {sortBy}. Valid options are: {string.Join(", ", Enum.GetNames(typeof(MovieSortOption)))}");
            }
            
            var (movies, totalCount) = await _movieService.GetSortedMoviesAsync(sortOption, ascending, page, pageSize);
            return Ok(new {movies,totalCount});
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
            var movies = await _movieService.GetFilteredMoviesAsync(genreId, startYear, endYear, qualityId); 
            return Ok(movies);
        }
        
         [HttpPost("recordView")]
        public async Task<IActionResult> RecordView([FromBody] int movieId) 
        {
            if (movieId <= 0)
            {
                return BadRequest("Invalid movie ID.");
            }

            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); 

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var authenticatedUserId))
            {
               
                return Unauthorized("User is not authenticated or User ID claim is missing.");
            }

            var userId = authenticatedUserId;

            try
            {
                await _movieService.RecordMovieViewAsync(movieId, userId);

                return NoContent();
            }
            catch (Exception ex)
            {

                if (ex.Message == "Movie not found") 
                {
                     return NotFound($"Movie with ID {movieId} not found.");
                }

                return StatusCode(500, "An error occurred while recording the view.");
            }
        }

      
        [HttpGet("{movieId}/totalViews")]
        public async Task<IActionResult> GetTotalViews(int movieId)
        {
            if (movieId <= 0)
            {
                return BadRequest("Invalid movie ID.");
            }

            try
            {
                var totalViews = await _movieService.GetTotalViewsAsync(movieId);

                return Ok(totalViews);
            }
            catch (Exception ex)
            {
                 
                return StatusCode(500, "Error retrieving total views.");
            }
        }

        [HttpGet("monthlyUniqueViews")]
        public async Task<IActionResult> GetAllMoviesMonthlyUniqueViews()
        {
            try
            {
                var uniqueViews = await _movieService.GetAllMoviesMonthlyUniqueViewsAsync();
                return Ok(uniqueViews);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error retrieving monthly unique views.");
            }
        }
    }
    
}
























