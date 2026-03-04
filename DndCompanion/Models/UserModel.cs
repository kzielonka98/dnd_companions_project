using Microsoft.AspNetCore.Identity;

namespace DndCompanion.Models
{
    public class UserModel : IdentityUser
    {
        public ICollection<CampaignModel> Campaigns { get; set; } = new List<CampaignModel>();

        public ICollection<UserCampaignModel> UserCampaigns { get; set; } = new List<UserCampaignModel>();

        public ICollection<CharacterModel> OwnedCharacters { get; set; } = new List<CharacterModel>();
    }
}
