using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class EventType
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        [MaxLength(100)]
        public required string Title { get; set; }

        [MaxLength(50)]
        public required string Slug { get; set; }

        public int DurationMinutes { get; set; }

        [MaxLength(100)]
        public required string LocationType { get; set; }

        [MaxLength(250)]
        public required string LocationValue { get; set; }

        public int BufferBeforeMinutes { get; set; }
        public int BufferAfterMinutes { get; set; }
        public int MaxDaysInAdvance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
