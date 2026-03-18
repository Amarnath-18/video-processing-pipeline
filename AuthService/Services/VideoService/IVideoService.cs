namespace AuthService.Services.VideoService
{
    public interface IVideoService
    {
        Task<string> DownloadAsync(string url);
    }
}
