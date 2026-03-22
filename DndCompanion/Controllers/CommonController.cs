using DndCompanion.Data.Services;
using DndCompanion.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DndCompanion.Controllers
{
    public class CommonController : Controller
    {
        protected readonly ICampaingsService _campaingsService;

        protected readonly ICharactersService _charactersService;

        public CommonController(
            ICampaingsService campaingsService,
            ICharactersService charactersService
        )
        {
            _campaingsService = campaingsService;
            _charactersService = charactersService;
        }

        protected async Task<UserModel> GetCurrentUser()
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return null;
            }
            return await HttpContext
                .RequestServices.GetService<UserManager<UserModel>>()
                .FindByNameAsync(User.Identity.Name);
        }

        protected async Task<UserModel> GetUserByUsername(string username)
        {
            return await HttpContext
                .RequestServices.GetService<UserManager<UserModel>>()
                .FindByNameAsync(username);
        }
    }
}
