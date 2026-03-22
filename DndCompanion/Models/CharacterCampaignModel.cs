namespace DndCompanion.Models
{
    public class CharacterCampaignModel
    {
        public int CharacterId { get; set; }

        public CharacterModel Character { get; set; } = null!;

        public int CampaignId { get; set; }

        public CampaignModel Campaign { get; set; } = null!;
    }
}
