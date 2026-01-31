using System.ComponentModel.DataAnnotations;

namespace DndCompanion.Models
{
    public class CampaignModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; } = null!;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; } = string.Empty;

        [Required]
        public bool Public { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public UserModel Owner { get; set; } = null!;

        public string OwnerId { get; set; } = null!;
    }
}
