using DndCompanion.Models;
using Microsoft.EntityFrameworkCore;

namespace DndCompanion.Data.Services
{
    public class CampaingsService : ICampaingsService
    {

        private readonly DndCompanionContext _context;

        public CampaingsService(DndCompanionContext context)
        {
            _context = context;
        }

        public async Task AddCampaignAsync(CampaignModel campaign)
        {
            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CampaignModel>> GetAllCampaignsAsync()
        {
            var campaigns = await _context.Campaigns.ToListAsync();
            return campaigns;
        }
    }
}