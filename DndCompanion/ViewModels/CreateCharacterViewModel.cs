using System.ComponentModel.DataAnnotations;

namespace DndCompanion.ViewModels
{
    public class CreateCharacterViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; } = string.Empty;

        public IFormFile? AvatarImage { get; set; } = null!;
    }
}