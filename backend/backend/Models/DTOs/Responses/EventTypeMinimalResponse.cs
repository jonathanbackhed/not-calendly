namespace backend.Models.DTOs.Responses
{
    public record EventTypeMinimalResponse
    {
        public Guid Id { get; init; }
        public required string Title { get; init; }
        public required string Slug { get; init; }
        public int DurationMinutes { get; init; }
        public required string LocationType { get; init; }
        public required string LocationValue { get; init; }
    }
}
