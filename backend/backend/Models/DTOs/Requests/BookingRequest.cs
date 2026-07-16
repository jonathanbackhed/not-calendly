using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Requests
{
    public record BookingRequest
    {
        [MaxLength(150)]
        public required string Name { get; init; }

        [EmailAddress]
        public required string Email { get; init; }

        [Phone]
        public string? PhoneNumber { get; init; }

        public string? Notes { get; init; }
    }
}
