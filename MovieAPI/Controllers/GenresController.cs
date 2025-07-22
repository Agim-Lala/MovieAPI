using Microsoft.AspNetCore.Mvc;
using MovieAPI.Domain.Genres;
using MovieAPI.Services;

namespace MovieAPI.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    
    public class GenresController : ControllerBase
    {
        private readonly GenresService _genresService;

        public GenresController(GenresService genresService)
        {
            _genresService = genresService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _genresService.GetAllGenresAsync();
            return Ok(genres);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenreById(int id)
        {
            var genre = await _genresService.GetGenreByIdAsync(id);
            if (genre == null) return NotFound();
            return Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenre([FromBody] CreateGenreDTO genreDto)
        {
            if (!ModelState.IsValid) return BadRequest();
            
            var createdGenre = await _genresService.CreateGenreAsync(genreDto);
            return CreatedAtAction(nameof(GetGenreById), new { id = createdGenre.Id }, createdGenre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] CreateGenreDTO genreDto)
        {
            if (!ModelState.IsValid) return BadRequest();
            var updatedGenre = await _genresService.CreateGenreAsync(genreDto);
            if (updatedGenre == null) return NotFound();
            return Ok(updatedGenre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var deletedGenre = await _genresService.DeleteGenreAsync(id);
            if (deletedGenre == null) return NotFound();
            return Ok(deletedGenre);
        }
    }


