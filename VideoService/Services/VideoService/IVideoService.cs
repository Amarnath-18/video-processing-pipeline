using YoutubeExplode.Common;

namespace VideoService.Services.VideoService
{
    public interface IVideoService
    {
        Task<(Stream Stream, string FileName, string ContentType)> DownloadAsync(string url, Resolution resolution);
    }
}
