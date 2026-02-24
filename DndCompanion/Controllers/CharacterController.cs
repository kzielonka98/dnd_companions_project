using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndCompanion.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace DndCompanion.Controllers
{
    public class CharacterController : CommonController
    {
        private readonly ICharactersService _charactersService;

        public CharacterController(ICharactersService charactersService)
        {
            _charactersService = charactersService;
        }

        public async Task<IActionResult> Index(int id)
        {
            var currentUser = await GetCurrentUser();
            var character = await _charactersService.GetCharacterByIdAsync(id);
            if (currentUser == null)
            {
                return View();
            }
            var CurrentUserCharacters = await _charactersService.GetCharactersByUserAsync(currentUser);

            if (CurrentUserCharacters.Any(c => c.Id == id))
            {
                return View(character);
            }

            return View();
        }
    }
}