using DndCompanion.Data.Services;
using DndCompanion.Models;
using DndCompanion.Models.SystemMessages.CampaingInvitation;
using DndCompanion.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DndCompanion.Controllers
{
    public class CampaignController : CommonController
    {
        public CampaignController(
            ICampaingsService campaingsService,
            ICharactersService charactersService
        )
            : base(campaingsService, charactersService) { }

        public async Task<IActionResult> Index(int id)
        {
            ViewBag.MaxNumberOfPlayersPerCampaign = Constants.MaxNumberOfPlayersPerCampaign;

            CampaignModel campaign = await _campaingsService.GetCampaignByIdAsync(id);
            IEnumerable<CharacterModel> characters =
                await _charactersService.GetCharactersByUserAsync(await GetCurrentUser());
            ViewBag.UserCharacters = characters;
            bool hasAccess = await HasAccessToCampaign(id);
            if (hasAccess)
            {
                return View(campaign);
            }
            else
            {
                return View();
            }
        }

        private async Task<bool> HasAccessToCampaign(int campaingId)
        {
            CampaignModel campaign = await _campaingsService.GetCampaignByIdAsync(campaingId);
            UserModel currentUser = await GetCurrentUser();

            // If the user is not logged in, they can only access public campaigns
            if (currentUser == null)
            {
                if (campaign != null && campaign.Public)
                {
                    return true;
                }
                return false;
            }

            // User has access to owned campaigns
            IEnumerable<CampaignModel> CurrentUserCampaigns =
                await _campaingsService.GetOwnedCampaignsByUserAsync(currentUser);

            // User has access to joined campaigns
            CurrentUserCampaigns = CurrentUserCampaigns.Concat(
                await _campaingsService.GetJoinedCampaignsByUserAsync(currentUser)
            );

            // User has access to campaigns they are invited to
            CurrentUserCampaigns = CurrentUserCampaigns.Concat(
                await _campaingsService.GetCampaignsByUserInvitationsAsync(currentUser)
            );

            if (campaign.Public || CurrentUserCampaigns.Any(c => c.Id == campaingId))
            {
                return true;
            }

            return false;
        }

        [Authorize]
        public async Task<IActionResult> AddUser(int campaignId)
        {
            ViewBag.CampaignId = campaignId;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddUser(
            AddUserToCampaingViewModel addUserToCampaingViewModel,
            int campaignId
        )
        {
            if (campaignId == 0)
            {
                return RedirectToAction("Index", "Campaigns");
            }

            ViewBag.CampaignId = campaignId;

            if (
                addUserToCampaingViewModel == null
                || string.IsNullOrEmpty(addUserToCampaingViewModel.UserName)
            )
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid username.");
                return View(addUserToCampaingViewModel);
            }

            UserModel user = await GetUserByUsername(addUserToCampaingViewModel.UserName);
            UserModel currentUser = await GetCurrentUser();

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(addUserToCampaingViewModel);
            }

            if (user == currentUser)
            {
                ModelState.AddModelError(string.Empty, "You cannot add yourself to the campaign.");
                return View(addUserToCampaingViewModel);
            }

            IEnumerable<CampaignInvitationModel> invitations =
                await _campaingsService.GetSentCampaignInvitationsForUserAsync(currentUser);

            if (invitations.Any(i => i.ReceiverId == user.Id && i.CampaignId == campaignId))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "You have already sent an invitation to this user for this campaign."
                );
                return View(addUserToCampaingViewModel);
            }

            CampaignModel campaign = await _campaingsService.GetCampaignByIdAsync(campaignId);

            if (campaign == null)
            {
                return RedirectToAction("Index", "Campaigns");
            }

            if (campaign.UsersCampaigns.Any(x => x.UserId == user.Id))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This user is already part of the campaign."
                );
                return View(addUserToCampaingViewModel);
            }

            if (
                campaign.UsersCampaigns.Count(x => !x.IsOwner)
                >= Constants.MaxNumberOfPlayersPerCampaign
            )
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This campaign has reached the maximum number of players."
                );
                return RedirectToAction("Index", new { id = campaignId });
            }

            await _campaingsService.CreateCampaingInvitationAsync(
                campaign,
                currentUser,
                user,
                addUserToCampaingViewModel.Content
            );
            addUserToCampaingViewModel.UserName = string.Empty;
            ModelState.AddModelError(
                string.Empty,
                $"Invitation to {user.UserName} sent successfully."
            );
            return View(addUserToCampaingViewModel);
        }

        [Authorize]
        public async Task<IActionResult> DeleteUser(string userId, int campaignId)
        {
            UserModel currentUser = await GetCurrentUser();
            IEnumerable<CampaignModel> ownedCampaings =
                await _campaingsService.GetOwnedCampaignsByUserAsync(currentUser);

            CampaignModel campaign = ownedCampaings.First(c => c.Id == campaignId);

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

        [Authorize]
        public async Task<IActionResult> LeaveCampaign(int campaignId)
        {
            UserModel currentUser = await GetCurrentUser();
            IEnumerable<CampaignModel> joinedCampaings =
                await _campaingsService.GetJoinedCampaignsByUserAsync(currentUser);

            CampaignModel campaign = joinedCampaings.First(c => c.Id == campaignId);

            if (campaign == null)
            {
                ModelState.AddModelError(string.Empty, "You are not part of this campaign.");
                return RedirectToAction("Index", "Campaigns");
            }

            await _campaingsService.DeleteUserFromCampaignAsync(campaignId, currentUser.Id);

            return RedirectToAction("Index", "Campaigns");
        }

        [Authorize]
        public async Task<IActionResult> CampaingInvitations()
        {
            UserModel currentUser = await GetCurrentUser();
            IEnumerable<CampaignInvitationModel> invitations =
                await _campaingsService.GetReceivedCampaignInvitationsForUserAsync(currentUser);
            invitations = invitations.Concat(
                await _campaingsService.GetSentCampaignInvitationsForUserAsync(currentUser)
            );
            return View(invitations);
        }

        [Authorize]
        public async Task<IActionResult> AcceptInvitation(int invitationId)
        {
            UserModel currentUser = await GetCurrentUser();
            IEnumerable<CampaignInvitationModel> invitations =
                await _campaingsService.GetReceivedCampaignInvitationsForUserAsync(currentUser);
            CampaignInvitationModel? invitation = invitations.FirstOrDefault(i =>
                i.Id == invitationId
            );

            if (invitation == null)
            {
                return RedirectToAction("CampaingInvitations");
            }
            await _campaingsService.AddPlayerToCampaignAsync(invitation.Campaign, currentUser);
            await _campaingsService.DeleteInvitationAsync(invitation.Id);

            return RedirectToAction("Index", new { id = invitation.Campaign.Id });
        }

        [Authorize]
        public async Task<IActionResult> CancelInvitation(int invitationId)
        {
            UserModel currentUser = await GetCurrentUser();
            IEnumerable<CampaignInvitationModel> invitations =
                await _campaingsService.GetSentCampaignInvitationsForUserAsync(currentUser);
            invitations.Concat(
                await _campaingsService.GetReceivedCampaignInvitationsForUserAsync(currentUser)
            );
            CampaignInvitationModel? invitation = invitations.FirstOrDefault(i =>
                i.Id == invitationId
            );

            if (invitation == null)
            {
                return RedirectToAction("CampaingInvitations");
            }

            await _campaingsService.DeleteInvitationAsync(invitation.Id);

            return RedirectToAction("CampaingInvitations");
        }

        [Authorize]
        public async Task<IActionResult> AssignCharacter(int campaignId, int characterId)
        {
            UserModel currentUser = await GetCurrentUser();
            CampaignModel campaign = await _campaingsService.GetCampaignByIdAsync(campaignId);
            CharacterModel character = await _charactersService.GetCharacterByIdAsync(characterId);

            // Check if the campaign and character exist
            if (campaign == null || character == null)
            {
                return RedirectToAction("Index");
            }

            // Check if the character belongs to the current user
            if (character.OwnerId != currentUser.Id)
            {
                ModelState.AddModelError(string.Empty, "You can only assign your own characters.");
                return RedirectToAction("Index", new { id = campaignId });
            }

            // Check if the user is part of the campaign
            if (!campaign.UsersCampaigns.Any(uc => uc.UserId == currentUser.Id))
            {
                ModelState.AddModelError(string.Empty, "You are not part of this campaign.");
                return RedirectToAction("Index", new { id = campaignId });
            }

            //Check if user already has a character assigned to the campaign
            if (campaign.Characters.Any(c => c.OwnerId == currentUser.Id))
            {
                ModelState.AddModelError(string.Empty, "You already have a character assigned to this campaign.");
                return RedirectToAction("Index", new { id = campaignId });
            }

            await _campaingsService.AssignCharacterToCampaignAsync(campaign, character);

            return RedirectToAction("Index", new { id = campaignId });
        }

        [Authorize]
        public async Task<IActionResult> UnassignCharacter(int campaignId, int characterId)
        {
            UserModel currentUser = await GetCurrentUser();
            CampaignModel campaign = await _campaingsService.GetCampaignByIdAsync(campaignId);
            CharacterModel character = await _charactersService.GetCharacterByIdAsync(characterId);

            // Check if the campaign and character exist
            if (campaign == null || character == null)
            {
                return RedirectToAction("Index");
            }

            // Check if the user is part of the campaign
            if (!campaign.UsersCampaigns.Any(uc => uc.UserId == currentUser.Id))
            {
                ModelState.AddModelError(string.Empty, "You are not part of this campaign.");
                return RedirectToAction("Index", new { id = campaignId });
            }

            // Check if the character belongs to the current user or user is owner of campaing
            bool isOwner = campaign.UsersCampaigns.FirstOrDefault(uc => uc.IsOwner).UserId == currentUser.Id;
            if(!isOwner && character.OwnerId != currentUser.Id)
            {
                ModelState.AddModelError(string.Empty, "You are not owner of this character or campaign");
                return RedirectToAction("Index", new { id = campaignId });
            }

            await _campaingsService.UnassignCharacterFromCampaignAsync(campaignId, characterId);

            return RedirectToAction("Index", new { id = campaignId });
        }
    }
}
