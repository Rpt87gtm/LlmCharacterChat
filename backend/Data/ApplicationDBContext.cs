using api.Models.User;
using llmChat.Models.Chat;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace llmChat.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public ApplicationDBContext(DbContextOptions dbContextOptions)
            :base(dbContextOptions)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            List<IdentityRole> roles = new()
            {
                new IdentityRole {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole {
                    Name = "User",
                    NormalizedName = "USER"
                },
            };
            builder.Entity<IdentityRole>().HasData(roles);

            builder.Entity<Character>()
               .HasIndex(c => new { c.Name, c.CreatedByAppUserId })
               .IsUnique();

            builder.Entity<Character>()
                .HasOne(c => c.CreatedByAppUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByAppUserId);

            builder.Entity<ChatHistory>()
                .HasOne(ch => ch.Character)
                .WithMany()
                .HasForeignKey(ch => ch.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ChatHistory>()
                .HasOne(ch => ch.AppUser)
                .WithMany()
                .HasForeignKey(ch => ch.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ChatHistory>()
                .HasIndex(ch => new { ch.AppUserId, ch.CharacterId })
                .IsUnique();

            builder.Entity<Message>()
                .HasOne(m => m.ChatHistory)
                .WithMany(ch => ch.Messages)
                .HasForeignKey(m => m.ChatHistoryId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
