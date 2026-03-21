namespace VideoService.Models.DownloadModels
{
    public record DownloadRequest
    {
        public required string Url { get; init; }
    }
}
