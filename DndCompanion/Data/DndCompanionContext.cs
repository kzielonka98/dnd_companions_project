using System;
using DndCompanion.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DndCompanion.Data{

    public class DndCompanionContext : IdentityDbContext<UserModel>
    {

        public DndCompanionContext(DbContextOptions<DndCompanionContext> options)
            : base(options)
        {
        }

        public DbSet<CampaignModel> Campaigns { get; set; }
    }
}