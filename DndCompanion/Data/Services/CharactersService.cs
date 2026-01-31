using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndCompanion.Models;
using Microsoft.EntityFrameworkCore;

namespace DndCompanion.Data.Services
{
    public class CharactersService : ICharactersService
    {
        private readonly DndCompanionContext _context;

        public CharactersService(DndCompanionContext context)
        {
            _context = context;
        }

        public async Task AddCharacterAsync(CharacterModel character, UserModel user)
        {
            _context.Characters.Add(character);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCharacterAsync(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character != null)
            {
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CharacterModel> GetCharacterByIdAsync(int id)
        {
            var character = await _context
                .Characters.Include(c => c.Owner)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            return character;
        }

        public async Task<IEnumerable<CharacterModel>> GetCharactersByUserAsync(UserModel user)
        {
            var characters = await _context
                .Users.Where(u => u.Id == user.Id)
                .Include(u => u.OwnedCharacters)
                .SelectMany(u => u.OwnedCharacters)
                .ToListAsync();
            return characters;
        }
    }
}
