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
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!?/\@#$%^&*]).+$", 
            ErrorMessage = "Password must contain at least one uppercase letter, one number, and one special character.")]
        public required string Password { get; init; }

        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug may only contain lowercase letters, numbers and hyphens.")]
        public required string Slug { get; init; }
    }
}
