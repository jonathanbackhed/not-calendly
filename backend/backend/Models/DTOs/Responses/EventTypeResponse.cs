namespace backend.Models.DTOs.Responses
{
    public record EventTypeResponse
    {
        public Guid Id { get; init; }
        public required string Title { get; init; }
        public required string Slug { get; init; }
        public int DurationMinutes { get; init; }
        public required string LocationType { get; init; }
        public required string LocationValue { get; init; }
        public int BufferBeforeMinutes { get; init; }
        public int BufferAfterMinutes { get; init; }
        public int MaxDaysInAdvance { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
