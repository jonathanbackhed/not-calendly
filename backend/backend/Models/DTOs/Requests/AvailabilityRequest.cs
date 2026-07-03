using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Requests
{
    public record AvailabilityRequest
    {
        [Range(0, 6, ErrorMessage = "DayOfWeek must be between 0 (Monday) and 6 (Sunday).")]
        public int DayOfWeek { get; init; }
        public TimeOnly StartTime { get; init; }
        public TimeOnly EndTime { get; init; }
        public bool IsActive { get; init; }
    }
}
