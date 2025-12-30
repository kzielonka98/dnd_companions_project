using System.ComponentModel.DataAnnotations;

namespace DndCompanion.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;
        
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        
        public string Password { get; set; } = null!;
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}