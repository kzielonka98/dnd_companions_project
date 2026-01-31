using DndCompanion.Data.Services;
using DndCompanion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DndCompanion.Controllers
{
    public class CampaignsController : CommonController
    {
        private readonly ICampaingsService _campaingsService;

        public CampaignsController(ICampaingsService campaingsService)
        {
            _campaingsService = campaingsService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();

            IEnumerable<CampaignModel> campaigns = await _campaingsService.GetCampaignsByUserAsync(
                user
            );
            return View(campaigns);
        }

        public async Task<IActionResult> PublicCampaigns()
        {
            IEnumerable<CampaignModel> campaigns =
                await _campaingsService.GetAllPublicCampaignsAsync();
            return View(campaigns);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CampaignModel model)
        {
            UserModel user = await GetCurrentUser();

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Something went wrong. Please try again later."
                );
                return View(model);
            }

            model.Owner = user;
            model.OwnerId = user.Id;

            await _campaingsService.AddCampaignAsync(model, user);
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await GetCurrentUser();
            IEnumerable<CampaignModel> campaigns = await _campaingsService.GetCampaignsByUserAsync(
                user
            );
            if (!campaigns.Any(c => c.Id == id))
            {
                return RedirectToAction("Index");
            }

            await _campaingsService.DeleteCampaignAsync(id);
            return RedirectToAction("Index");
        }
    }
}
