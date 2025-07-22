using Microsoft.AspNetCore.Mvc;
using MovieAPI.Services;

namespace MovieAPI.Controllers;


    [ApiController]
    [Route("api/[controller]")]
    public class QualityController : ControllerBase
    {
        private readonly QualityService _qualityService;

        public QualityController(QualityService qualityService)
        {
            _qualityService = qualityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQualities()
        {
            var qualities = await _qualityService.GetAllQualitiesAsync();
            return Ok(qualities);
        }
}