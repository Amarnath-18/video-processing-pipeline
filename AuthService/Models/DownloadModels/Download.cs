namespace AuthService.Models.DownloadModels
{
    public record DownloadRequest
    {
        public string Url { get; init; }
    }
}
