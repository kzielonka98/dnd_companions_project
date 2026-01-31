using DndCompanion.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace DndCompanion.Controllers
{
    public class CampaignController : CommonController
    {
        private readonly ICampaingsService _campaingsService;

        public CampaignController(ICampaingsService campaingsService)
        {
            _campaingsService = campaingsService;
        }

        public async Task<IActionResult> Index(int id)
        {
            var currentUser = await GetCurrentUser();
            var campaign = await _campaingsService.GetCampaignByIdAsync(id);
            if (currentUser == null)
            {
                if (campaign != null && campaign.Public)
                {
                    return View(campaign);
                }
                return View();
            }
            var CurrentUserCampaigns = await _campaingsService.GetCampaignsByUserAsync(currentUser);

            if (campaign.Public || CurrentUserCampaigns.Any(c => c.Id == id))
            {
                return View(campaign);
            }

            return View();
        }
    }
}
