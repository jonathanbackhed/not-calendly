using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public required string Token { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        public Guid UserId { get; set; }
        public required User User { get; set; }
    }
}
