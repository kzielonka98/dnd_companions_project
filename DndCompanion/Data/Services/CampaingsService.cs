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

        public async Task AddCampaignAsync(CampaignModel campaign, UserModel user)
        {
            _context.Campaigns.Add(campaign);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CampaignModel>> GetAllPublicCampaignsAsync()
        {
            var campaigns = await _context
                .Campaigns.Where(c => c.Public)
                .Include(c => c.Owner)
                .Select(c => c)
                .ToListAsync();
            return campaigns;
        }

        public async Task<IEnumerable<CampaignModel>> GetCampaignsByUserAsync(UserModel user)
        {
            var campaigns = await _context
                .Users.Where(u => u.Id == user.Id)
                .Include(u => u.OwnedCampaings)
                .SelectMany(u => u.OwnedCampaings)
                .ToListAsync();
            return campaigns;
        }

        public async Task DeleteCampaignAsync(int id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign != null)
            {
                _context.Campaigns.Remove(campaign);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CampaignModel> GetCampaignByIdAsync(int id)
        {
            var campaign = await _context
                .Campaigns.Include(c => c.Owner)
                .Where(c => c.Id == id)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            return campaign;
        }
    }
}
