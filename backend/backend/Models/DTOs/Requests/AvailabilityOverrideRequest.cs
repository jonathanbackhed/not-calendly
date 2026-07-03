namespace backend.Models.DTOs.Requests
{
    public record AvailabilityOverrideRequest
    {
        public DateOnly OverrideDate { get; init; }
        public bool IsBlocked { get; init; }
        public TimeOnly? StartTime { get; init; }
        public TimeOnly? EndTime { get; init; }
    }
}
