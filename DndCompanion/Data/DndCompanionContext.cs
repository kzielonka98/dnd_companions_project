using DndCompanion.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DndCompanion.Data
{
    public class DndCompanionContext : IdentityDbContext<UserModel>
    {
        public DndCompanionContext(DbContextOptions<DndCompanionContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserModel>()
                .HasMany(x => x.Campaigns)
                .WithMany(x => x.Users)
                .UsingEntity<UserCampaignModel>();

            builder
                .Entity<CampaignModel>()
                .HasMany(c => c.Users)
                .WithMany(u => u.Campaigns)
                .UsingEntity<UserCampaignModel>();

            builder
                .Entity<CharacterModel>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.OwnedCharacters)
                .HasForeignKey(c => c.OwnerId)
                .IsRequired();
        }

        public DbSet<CampaignModel> Campaigns { get; set; }
        public DbSet<CharacterModel> Characters { get; set; }
        public DbSet<UserCampaignModel> UserCampaigns { get; set; }
    }
}
