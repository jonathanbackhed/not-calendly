using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AvailabilityRule> AvailabilityRules { get; set; }
        public DbSet<AvailabilityOverride> AvailabilityOverrides { get; set; }
        public DbSet<EventType> EventTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Slug).IsUnique();
            });

            modelBuilder.Entity<AvailabilityRule>()
                .HasIndex(a => new { a.UserId, a.DayOfWeek })
                .IsUnique();

            modelBuilder.Entity<AvailabilityOverride>()
                .HasIndex(a => new { a.UserId, a.OverrideDate })
                .IsUnique();

            modelBuilder.Entity<EventType>()
                .HasIndex(a => new { a.UserId, a.Slug })
                .IsUnique();
        }
    }
}
