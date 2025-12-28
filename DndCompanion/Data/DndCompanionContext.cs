using System;
using DndCompanion.Models;
using Microsoft.EntityFrameworkCore;

namespace DndCompanion.Data{

    public class DndCompanionContext : DbContext
    {

        public DndCompanionContext(DbContextOptions<DndCompanionContext> options)
            : base(options)
        {
        }

        public DbSet<CampaignModel> Campaigns { get; set; }
    }
}