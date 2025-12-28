using DndCompanion.Data;
using DndCompanion.Data.Services;
using DndCompanion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DndCompanion.Controllers
{
    public class CampaignsController : Controller
    {
        private readonly ICampaingsService _campaingsService;

        public CampaignsController(ICampaingsService campaingsService)
        {
            _campaingsService = campaingsService;
        }

        public async Task<IActionResult> Index()
        {
            var campaigns = await _campaingsService.GetAllCampaignsAsync();
            return View(campaigns);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CampaignModel campaign)
        {
            if (ModelState.IsValid)
            {
                await _campaingsService.AddCampaignAsync(campaign);
                return RedirectToAction("Index");
            }
            return View(campaign);
        }
    }
}