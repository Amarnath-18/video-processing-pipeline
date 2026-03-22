namespace Common
{
    public record CommonResponse
    {
        public int Status { get; init; }
        public string? Message { get; init; }
    }
}
