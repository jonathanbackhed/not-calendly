using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class AvailabilityRules
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public required int DayOfWeek { get; set; }

        [Required]
        public required TimeOnly StartTime { get; set; }

        [Required]
        public required TimeOnly EndTime { get; set; }
        
        [Required]
        public required bool IsActive { get; set; }

        [Required]
        public required DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
