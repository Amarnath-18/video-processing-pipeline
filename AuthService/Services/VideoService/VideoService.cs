using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace AuthService.Services.VideoService
{
    public class VideoService : IVideoService
    {
        public async Task<string> DownloadAsync(string url)
        {
            var youtube = new YoutubeClient();

            // Get video
            var video = await youtube.Videos.GetAsync(url);

            // Get available streams
            var manifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);

            // Pick best quality (video + audio)
            var streamInfo = manifest
                .GetMuxedStreams()
                .GetWithHighestVideoQuality();

            if (streamInfo == null)
                throw new Exception("No suitable stream found");

            // Save file
            var fileName = $"{video.Id}.mp4";
            var filePath = Path.Combine("Downloads", fileName);

            Directory.CreateDirectory("Downloads");

            await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);

            return filePath;
        }
    }
}
