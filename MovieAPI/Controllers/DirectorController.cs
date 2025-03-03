using Microsoft.AspNetCore.Mvc;
using MovieAPI.Domain.Directors;
using MovieAPI.Services;

namespace MovieAPI.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class DirectorController : ControllerBase
    {
        private readonly DirectorService _directorService;

        public DirectorController(DirectorService directorService)
        {
            _directorService = directorService;
        }

        // GET: api/director
        [HttpGet]
        public async Task<ActionResult<List<DirectorDTO>>> GetAllDirectors()
        {
            var directors = await _directorService.GetAllDirectorsAsync();
            return Ok(directors);
        }

        // GET: api/director/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DirectorDTO>> GetDirectorById(int id)
        {
            var director = await _directorService.GetDirectorByIdAsync(id);

            if (director == null)
            {
                return NotFound(); // Return 404 if director is not found
            }

            return Ok(director);
        }

        // POST: api/director
        [HttpPost]
        public async Task<ActionResult<DirectorDTO>> CreateDirector([FromBody] CreateDirectorDTO createDirectorDTO)
        {
            if (createDirectorDTO == null)
            {
                return BadRequest("Director data is required.");
            }

            var directorDTO = await _directorService.CreateDirectorAsync(createDirectorDTO);

            return CreatedAtAction(nameof(GetDirectorById), new { id = directorDTO.DirectorId }, directorDTO);
        }

        // PUT: api/director/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<DirectorDTO>> UpdateDirector(int id, [FromBody] DirectorDTO directorDTO)
        {
            if (directorDTO == null)
            {
                return BadRequest("Director data is required.");
            }

            var updatedDirector = await _directorService.UpdateDirectorAsync(id, directorDTO);

            if (updatedDirector == null)
            {
                return NotFound(); // Return 404 if director is not found
            }

            return Ok(updatedDirector);
        }

        // DELETE: api/director/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDirector(int id)
        {
            var deleted = await _directorService.DeleteDirectorAsync(id);

            if (!deleted)
            {
                return NotFound(); // Return 404 if director is not found
            }

            return NoContent(); // Return 204 No Content to indicate successful deletion
        }
    }
