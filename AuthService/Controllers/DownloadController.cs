using Microsoft.AspNetCore.Mvc;
using AuthService.Services.VideoService;
using AuthService.Models.DownloadModels;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DownloadController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public DownloadController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpPost]
        public async Task<IActionResult> Download([FromBody] DownloadRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("URL is required");

            try
            {
                var filePath = await _videoService.DownloadAsync(request.Url);

                return Ok(new
                {
                    message = "Download successful",
                    path = filePath
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Download failed",
                    error = ex.Message
                });
            }
        }
    }
}
