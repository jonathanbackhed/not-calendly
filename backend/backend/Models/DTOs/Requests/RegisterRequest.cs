using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Requests
{
    public record RegisterRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [MaxLength(50)]
        public required string Username { get; init; }

        [Required]
        [MinLength(10)]
        public required string Password { get; init; }

        [Required]
        [MaxLength(50)]
        public required string Slug { get; init; }
    }
}
