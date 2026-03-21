using Microsoft.Extensions.Logging;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace VideoService.Services.VideoService
{
    public class VideoService : IVideoService
    {
        private readonly ILogger<VideoService> _logger;

        public VideoService(ILogger<VideoService> logger)
        {
            _logger = logger;
        }

        public async Task<(Stream Stream, string FileName, string ContentType)> DownloadAsync(string url)
        {
            _logger.LogInformation("Starting video download for URL: {Url}", url);

            var youtube = new YoutubeClient();

            var video = await youtube.Videos.GetAsync(url);
            _logger.LogInformation("Fetched video metadata. VideoId: {VideoId}", video.Id);

            var manifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);

            var streamInfo = manifest
                .GetMuxedStreams()
                .GetWithHighestVideoQuality();

            if (streamInfo == null)
            {
                _logger.LogError("No suitable muxed stream found for VideoId: {VideoId}", video.Id);
                throw new Exception("No suitable stream found");
            }

            var extension = streamInfo.Container.Name;
            var fileName = $"{video.Id}.{extension}";
            var contentType = extension;

            _logger.LogInformation("Downloading video to response stream. FileName: {FileName}", fileName);

            var outputStream = new MemoryStream();
            await using (var inputStream = await youtube.Videos.Streams.GetAsync(streamInfo))
            {
                await inputStream.CopyToAsync(outputStream);
            }
            outputStream.Position = 0;
            _logger.LogInformation("Video downloaded successfully. FileName: {FileName}", fileName);

            return (outputStream, fileName, contentType);
        }
    }
}
