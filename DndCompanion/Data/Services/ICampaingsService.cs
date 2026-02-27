using DndCompanion.Models;

namespace DndCompanion.Data.Services
{
    public interface ICampaingsService
    {
        Task<IEnumerable<CampaignModel>> GetAllPublicCampaignsAsync();

        Task AddCampaignAsync(CampaignModel campaign, UserModel user);

        Task AddPlayerToCampaignAsync(CampaignModel campaign, UserModel user);

        Task<IEnumerable<CampaignModel>> GetOwnedCampaignsByUserAsync(UserModel user);

        Task<CampaignModel> GetCampaignByIdAsync(int id);

        Task DeleteCampaignAsync(int id);
    }
}