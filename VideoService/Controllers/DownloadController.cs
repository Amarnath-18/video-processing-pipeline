using Microsoft.AspNetCore.Mvc;
using VideoService.Models.DownloadModels;
using VideoService.Services.VideoService;
using YoutubeExplode.Common;
using Common;

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
        public async Task<IActionResult> Download([FromBody] DownloadRequest request , [FromQuery] Resolution resolution)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
                return BadRequest(new CommonResponse
                {
                    Status = AppStatusCode.BAD_REQUEST,
                    Message = "Url cannot be empty!!"
                });

            try
            {
                var result = await _videoService.DownloadAsync(request.Url, resolution);

                return File(result.Stream, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new CommonResponse
                {
                    Status = AppStatusCode.INTERNAL_SERVER_ERROR,
                    Message = "Download failed",
                });
            }
        }
    }
}
