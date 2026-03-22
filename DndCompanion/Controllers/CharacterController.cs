using DndCompanion.Data.Services;
using DndCompanion.Models;
using Microsoft.AspNetCore.Mvc;

namespace DndCompanion.Controllers
{
    public class CharacterController : CommonController
    {
        public CharacterController(
            ICampaingsService campaingsService,
            ICharactersService charactersService
        )
            : base(campaingsService, charactersService) { }

        public async Task<IActionResult> Index(int id)
        {
            var currentUser = await GetCurrentUser();
            var character = await _charactersService.GetCharacterByIdAsync(id);
            if (
                currentUser == null
                || character == null
                || !HasAccessToCharacter(currentUser, character)
            )
            {
                return View();
            }

            return View(character);
        }

        private bool HasAccessToCharacter(UserModel user, CharacterModel character)
        {
            // Check if the user is the owner of the character
            if (character.OwnerId == user.Id)
            {
                return true;
            }

            // Check if the user is part of any campaign that the character is assigned to
            if (
                character.CharacterCampaigns.Any(cc =>
                    cc.Campaign.Users.Any(uc => uc.Id == user.Id)
                )
            )
            {
                return true;
            }

            return false;
        }
    }
}
