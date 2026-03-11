using DndCompanion.Models;
using DndCompanion.Models.SystemMessages.CampaingInvitation;

namespace DndCompanion.Data.Services
{
    public interface ICampaingsService
    {
        Task<IEnumerable<CampaignModel>> GetAllPublicCampaignsAsync();

        Task AddCampaignAsync(CampaignModel campaign, UserModel user);

        Task DeleteCampaignAsync(int id);

        Task AddPlayerToCampaignAsync(CampaignModel campaign, UserModel user);

        Task CreateCampaingInvitationAsync(
            CampaignModel campaign,
            UserModel sender,
            UserModel receiver,
            string content = null
        );

        Task<IEnumerable<CampaignInvitationModel>> GetReceivedCampaignInvitationsForUserAsync(
            UserModel user
        );

        Task<IEnumerable<CampaignInvitationModel>> GetSentCampaignInvitationsForUserAsync(
            UserModel user
        );

        Task DeleteUserFromCampaignAsync(int campaignId, string userId);

        Task<IEnumerable<CampaignModel>> GetOwnedCampaignsByUserAsync(UserModel user);

        Task<IEnumerable<CampaignModel>> GetJoinedCampaignsByUserAsync(UserModel user);

        Task<CampaignModel> GetCampaignByIdAsync(int id);

        Task<IEnumerable<CampaignModel>> GetCampaignsByUserInvitationsAsync(UserModel user);

        Task DeleteInvitationAsync(int invitationId);
    }
}
