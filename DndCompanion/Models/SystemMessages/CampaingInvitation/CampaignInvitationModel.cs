using System.ComponentModel.DataAnnotations;

namespace DndCompanion.Models.SystemMessages.CampaingInvitation
{
    public class CampaignInvitationModel : SystemMessageModel
    {
        [Required]
        public CampaignModel Campaign { get; set; } = null!;

        public int CampaignId { get; set; }

        public override string Title => $"Invitation to join campaign '{Campaign.Name}'";
    }
}