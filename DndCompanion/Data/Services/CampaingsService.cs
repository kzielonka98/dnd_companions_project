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
            UserCampaignModel userCampaign = new()
            {
                UserId = user.Id,
                User = user,
                CampaignId = campaign.Id,
                Campaign = campaign,
                IsOwner = true,
            };
            _context.UserCampaigns.Add(userCampaign);
            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCampaignAsync(int id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign != null)
            {
                _context.UserCampaigns.RemoveRange(
                    _context.UserCampaigns.Where(uc => uc.CampaignId == id)
                );
                _context.Campaigns.Remove(campaign);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddPlayerToCampaignAsync(CampaignModel campaign, UserModel user)
        {
            UserCampaignModel userCampaign = new()
            {
                UserId = user.Id,
                User = user,
                CampaignId = campaign.Id,
                Campaign = campaign,
                IsOwner = false,
            };
            _context.UserCampaigns.Add(userCampaign);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserFromCampaignAsync(int campaignId, string userId)
        {
            var userCampaign = await _context.UserCampaigns
                .Where(uc => uc.CampaignId == campaignId && uc.UserId == userId)
                .FirstOrDefaultAsync();
            if (userCampaign != null)
            {
                _context.UserCampaigns.Remove(userCampaign);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CampaignModel>> GetAllPublicCampaignsAsync()
        {
            var campaigns = await _context
                .Campaigns.Where(c => c.Public)
                .Include(c => c.Users)
                .Select(c => c)
                .ToListAsync();
            return campaigns;
        }

        public async Task<IEnumerable<CampaignModel>> GetOwnedCampaignsByUserAsync(UserModel user)
        {
            var campaigns = await _context
                .Users.Where(u => u.Id == user.Id)
                .SelectMany(u => u.Campaigns)
                .Where(c => c.UsersCampaigns.Any(uc => uc.UserId == user.Id && uc.IsOwner))
                .Include(c => c.Users)
                .ToListAsync();
            return campaigns;
        }

        public async Task<IEnumerable<CampaignModel>> GetJoinedCampaignsByUserAsync(UserModel user)
        {
            var campaigns = await _context
                .Users.Where(u => u.Id == user.Id)
                .SelectMany(u => u.Campaigns)
                .Where(c => c.UsersCampaigns.Any(uc => uc.UserId == user.Id && !uc.IsOwner))
                .Include(c => c.Users)
                .ToListAsync();
            return campaigns;
        }

        public async Task<CampaignModel> GetCampaignByIdAsync(int id)
        {
            var campaign = await _context
                .Campaigns.Where(c => c.Id == id)
                .Include(c => c.Users)
                .Where(us => us.UsersCampaigns.Any(uc => uc.CampaignId == id))
                .FirstOrDefaultAsync();
            return campaign;
        }
    }
}
