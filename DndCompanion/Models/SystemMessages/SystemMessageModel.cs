using System.ComponentModel.DataAnnotations;

namespace DndCompanion.Models.SystemMessages
{
    public abstract class SystemMessageModel
    {
        public int Id { get; set; }

        [Required]
        public UserModel Receiver { get; set; } = null!;

        public string ReceiverId { get; set; } = null!;

        [Required]
        public UserModel Sender { get; set; } = null!;

        public string? SenderId { get; set; } = null!;

        [Required]
        public abstract string Title { get; }

        [Required]
        public string Content { get; set; } = null!;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
