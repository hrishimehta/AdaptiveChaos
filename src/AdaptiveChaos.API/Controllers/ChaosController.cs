using AdaptiveChaos.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdaptiveChaos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChaosController : ControllerBase
    {
        private readonly ChuckNorrisService _chuckNorrisService;
        public ChaosController(ChuckNorrisService chuckNorrisService)
        {
            _chuckNorrisService = chuckNorrisService;
        }

        [HttpGet("random-joke")]
        public async Task<IActionResult> GetRandomJoke()
        {
            try
            {
                var joke = await _chuckNorrisService.GetRandomJoke();
                return Ok(joke.Value);
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                return StatusCode(500, ex.Message);
            }
        }
    }
}
