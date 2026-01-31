using DndCompanion.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DndCompanion.Controllers
{
    public class CommonController : Controller
    {
        public async Task<UserModel> GetCurrentUser()
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return null;
            }
            return await HttpContext
                .RequestServices.GetService<UserManager<UserModel>>()
                .FindByNameAsync(User.Identity.Name);
        }
    }
}
