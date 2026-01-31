using System.Text.RegularExpressions;
using DndCompanion.Models;
using DndCompanion.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DndCompanion.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<UserModel> _signInManager;

        private readonly UserManager<UserModel> _userManager;

        private const string emailRegex = @"[^@ \t\r\n]+@[^@ \t\r\n]+\.[^@ \t\r\n]+";

        public AccountController(
            SignInManager<UserModel> signInManager,
            UserManager<UserModel> userManager
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result;

                if (!IsEmail(model.EmailorUsername))
                {
                    result = await _signInManager.PasswordSignInAsync(
                        model.EmailorUsername,
                        model.Password,
                        model.RememberMe,
                        false
                    );
                }
                else
                {
                    var user = await _userManager.FindByEmailAsync(model.EmailorUsername);
                    if (user != null)
                    {
                        result = await _signInManager.PasswordSignInAsync(
                            user.UserName,
                            model.Password,
                            model.RememberMe,
                            false
                        );
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Email or password is incorrect");
                        return View(model);
                    }
                }

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email or password is incorrect");
                    return View(model);
                }
            }

            return View(model);
        }

        private static bool IsEmail(string EmailorUsername)
        {
            return Regex.IsMatch(EmailorUsername, emailRegex);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel user = new UserModel { UserName = model.Username, Email = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }
            }

            return View(model);
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email not found");
                    return View(model);
                }
                else
                {
                    return RedirectToAction(
                        "ChangePassword",
                        "Account",
                        new { email = user.Email }
                    );
                }
            }
            return View(model);
        }

        public IActionResult ChangePassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email not found");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Something went wrong. Try Again.");
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
