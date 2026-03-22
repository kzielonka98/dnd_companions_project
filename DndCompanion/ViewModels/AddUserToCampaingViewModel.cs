using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DndCompanion.ViewModels
{
    public class AddUserToCampaingViewModel
    {
        [Required]
        public string UserName { get; set; } = null!;

        [MaxLength(300, ErrorMessage = "Content cannot exceed 300 characters.")]
        public string? Content { get; set; } = null;
    }
}