using backend.Enums;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class Booking
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid EventTypeId { get; set; }
        public EventType? EventType { get; set; }

        public required string GuestName { get; set; }

        [EmailAddress]
        public required string GuestEmail { get; set; }

        [Phone]
        public string? GuestPhone { get; set; }

        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }

        public BookingStatus Status { get; set; }
        public required string CancelToken { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
