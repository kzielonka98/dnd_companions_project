using Microsoft.AspNetCore.Identity;

namespace DndCompanion.Models
{
    public class UserModel : IdentityUser
    {
        public ICollection<CampaignModel> OwnedCampaings { get; set; } = new List<CampaignModel>();

        public ICollection<CharacterModel> OwnedCharacters { get; set; } = new List<CharacterModel>();
    }
}
