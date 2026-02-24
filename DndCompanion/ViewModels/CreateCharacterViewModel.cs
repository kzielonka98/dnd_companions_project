using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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