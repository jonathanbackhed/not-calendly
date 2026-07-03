namespace backend.Models.DTOs.Responses
{
    public record AvailabilityOverrideResponse
    {
        public Guid Id { get; init; }
        public DateOnly OverrideDate { get; init; }
        public bool IsBlocked { get; init; }
        public TimeOnly? StartTime { get; init; }
        public TimeOnly? EndTime { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
