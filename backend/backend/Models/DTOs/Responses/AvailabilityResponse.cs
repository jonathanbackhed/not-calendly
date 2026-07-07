namespace backend.Models.DTOs.Responses
{
    public record AvailabilityResponse
    {
        public Guid Id { get; init; }
        public int DayOfWeek { get; init; }
        public TimeOnly StartTime { get; init; }
        public TimeOnly EndTime { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
