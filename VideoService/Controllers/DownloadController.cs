using Microsoft.AspNetCore.Mvc;
using VideoService.Models.DownloadModels;
using VideoService.Services.VideoService;

namespace VideoService.Controllers
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
                var result = await _videoService.DownloadAsync(request.Url);
                return File(result.Stream, result.ContentType, result.FileName);
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
