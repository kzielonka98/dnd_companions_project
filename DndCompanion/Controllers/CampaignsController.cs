using DndCompanion.Data.Services;
using DndCompanion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DndCompanion.Controllers
{
    public class CampaignsController : CommonController
    {
        public CampaignsController(
            ICampaingsService campaingsService,
            ICharactersService charactersService
        )
            : base(campaingsService, charactersService) { }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();

            ViewBag.MaxNumberOfCampaignsPerUser = Constants.MaxNumberOfCampaignsPerUser;

            IEnumerable<CampaignModel> campaigns =
                await _campaingsService.GetOwnedCampaignsByUserAsync(user);

            campaigns = campaigns.Concat(
                await _campaingsService.GetJoinedCampaignsByUserAsync(user)
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
            var UserCampaigns = await _campaingsService.GetOwnedCampaignsByUserAsync(user);
            if (UserCampaigns.Count() >= Constants.MaxNumberOfCampaignsPerUser)
            {
                return RedirectToAction("Index");
            }
            await _campaingsService.AddCampaignAsync(model, user);
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await GetCurrentUser();
            IEnumerable<CampaignModel> campaigns =
                await _campaingsService.GetOwnedCampaignsByUserAsync(user);
            if (!campaigns.Any(c => c.Id == id))
            {
                return RedirectToAction("Index");
            }

            await _campaingsService.DeleteCampaignAsync(id);
            return RedirectToAction("Index");
        }
    }
}
