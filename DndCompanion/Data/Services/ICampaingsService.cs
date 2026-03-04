using DndCompanion.Models;

namespace DndCompanion.Data.Services
{
    public interface ICampaingsService
    {
        Task<IEnumerable<CampaignModel>> GetAllPublicCampaignsAsync();
        
        Task AddCampaignAsync(CampaignModel campaign, UserModel user);

        Task DeleteCampaignAsync(int id);

        Task AddPlayerToCampaignAsync(CampaignModel campaign, UserModel user);

        Task DeleteUserFromCampaignAsync(int campaignId, string userId);

        Task<IEnumerable<CampaignModel>> GetOwnedCampaignsByUserAsync(UserModel user);

        Task<IEnumerable<CampaignModel>> GetJoinedCampaignsByUserAsync(UserModel user);

        Task<CampaignModel> GetCampaignByIdAsync(int id);


    }
}