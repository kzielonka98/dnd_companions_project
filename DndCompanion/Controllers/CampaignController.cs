using DndCompanion.Data.Services;
using DndCompanion.Models;
using Microsoft.AspNetCore.Authorization;
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
            ViewBag.MaxNumberOfPlayersPerCampaign = Constants.MaxNumberOfPlayersPerCampaign;

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
            var CurrentUserCampaigns = await _campaingsService.GetOwnedCampaignsByUserAsync(
                currentUser
            );

            CurrentUserCampaigns = CurrentUserCampaigns.Concat(
                await _campaingsService.GetJoinedCampaignsByUserAsync(currentUser)
            );

            if (campaign.Public || CurrentUserCampaigns.Any(c => c.Id == id))
            {
                return View(campaign);
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> AddUser(int campaignId)
        {
            ViewBag.CampaignId = campaignId;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddUser(UserModel userModel, int campaignId)
        {
            if (campaignId == 0)
            {
                return RedirectToAction("Index", "Campaigns");
            }

            ViewBag.CampaignId = campaignId;

            if (userModel == null || string.IsNullOrEmpty(userModel.UserName))
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid username.");
                return View(userModel);
            }

            var user = await GetUserByUsername(userModel.UserName);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(userModel);
            }

            if (user == await GetCurrentUser())
            {
                ModelState.AddModelError(string.Empty, "You cannot add yourself to the campaign.");
                return View(userModel);
            }

            var campaign = await _campaingsService.GetCampaignByIdAsync(campaignId);

            if (campaign == null)
            {
                return RedirectToAction("Index", "Campaigns");
            }

            if (campaign.UsersCampaigns.Any(x => x.UserId == user.Id))
            {
                ModelState.AddModelError(string.Empty, "This user is already part of the campaign.");
                return View(userModel);
            }

            if (campaign.UsersCampaigns.Count(x => !x.IsOwner) >= Constants.MaxNumberOfPlayersPerCampaign)
            {
                ModelState.AddModelError(string.Empty, "This campaign has reached the maximum number of players.");
                return RedirectToAction("Index", new { id = campaignId });
            }

            await _campaingsService.AddPlayerToCampaignAsync(campaign, user);

            return RedirectToAction("Index", new { id = campaignId });
        }

        [Authorize]
        public async Task<IActionResult> DeleteUser(string userId, int campaignId)
        {
            var currentUser = await GetCurrentUser();
            var ownedCampaings = await _campaingsService.GetOwnedCampaignsByUserAsync(currentUser);

            var campaign = ownedCampaings.First(c => c.Id == campaignId);

            if (campaign == null)
            {
                ModelState.AddModelError(string.Empty, "You are not the owner of this campaign.");
                return RedirectToAction("Index", new { id = campaignId });
            }

            if (campaign.UsersCampaigns.All(x => x.UserId != userId))
            {
                ModelState.AddModelError(string.Empty, "This user is not part of the campaign.");
                return RedirectToAction("Index", new { id = campaignId });
            }

            await _campaingsService.DeleteUserFromCampaignAsync(campaignId, userId);

            return RedirectToAction("Index", new { id = campaignId });
        }
    }
}
