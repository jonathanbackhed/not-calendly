using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class AvailabilityOverride
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public DateOnly OverrideDate { get; set; }
        public bool IsBlocked { get; set; } // full day
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
