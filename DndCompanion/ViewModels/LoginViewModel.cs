using System.ComponentModel.DataAnnotations;

namespace DndCompanion.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Email or Username")]
        public string EmailorUsername { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
