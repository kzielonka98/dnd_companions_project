using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndCompanion.Data.Services;
using DndCompanion.Models;
using DndCompanion.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DndCompanion.Controllers
{
    public class CharactersController : CommonController
    {
        private readonly ICharactersService _charactersService;

        public CharactersController(ICharactersService charactersService)
        {
            _charactersService = charactersService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();

            IEnumerable<CharacterModel> characters =
                await _charactersService.GetCharactersByUserAsync(user);
            return View(characters);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCharacterViewModel model)
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
            CharacterModel character = new CharacterModel
            {
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.Now,
                Owner = user,
                OwnerId = user.Id,
            };

            if (model.AvatarImage != null && model.AvatarImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.AvatarImage.CopyToAsync(memoryStream);

                    // Upload the file if less than 10 MiB
                    if (memoryStream.Length < 10485760)
                    {
                        character.AvatarImage = memoryStream.ToArray();
                    }
                    else
                    {
                        ModelState.AddModelError("File",
                            "The file is too large. Maximum allowed size is 10 MiB.");
                        return View(model);
                    }
                }
            }
            await _charactersService.AddCharacterAsync(character, user);
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await GetCurrentUser();
            IEnumerable<CharacterModel> characters =
                await _charactersService.GetCharactersByUserAsync(user);
            if (!characters.Any(c => c.Id == id))
            {
                return RedirectToAction("Index");
            }

            await _charactersService.DeleteCharacterAsync(id);
            return RedirectToAction("Index");
        }
    }
}
