using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Requests
{
    public record EventTypeRequest
    {
        [MaxLength(100)]
        public required string Title { get; init; }

        [MaxLength(50)]
        public required string Slug { get; init; }

        [Range(15, 720)]
        public int DurationMinutes { get; init; }

        [MaxLength(100)]
        public required string LocationType { get; init; }

        [MaxLength(250)]
        public required string LocationValue { get; init; }

        [Range(0, 240)]
        public int BufferBeforeMinutes { get; init; }

        [Range(0, 240)]
        public int BufferAfterMinutes { get; init; }

        [Range(1, 365)]
        public int MaxDaysInAdvance { get; init; }
        public bool IsActive { get; init; }
    }
}
