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

            builder
                .Entity<CampaignModel>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.OwnedCampaings)
                .HasForeignKey(c => c.OwnerId)
                .IsRequired();
        }

        public DbSet<CampaignModel> Campaigns { get; set; }
    }
}
