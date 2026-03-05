using DndCompanion.Models.SystemMessages;
using Microsoft.AspNetCore.Identity;

namespace DndCompanion.Models
{
    public class UserModel : IdentityUser
    {
        public ICollection<CampaignModel> Campaigns { get; set; } = [];

        public ICollection<UserCampaignModel> UserCampaigns { get; set; } = [];

        public ICollection<CharacterModel> OwnedCharacters { get; set; } = [];

        public ICollection<SystemMessageModel> ReceivedInvitations { get; set; } = [];

        public ICollection<SystemMessageModel> SentInvitations { get; set; } = [];
    }
}
