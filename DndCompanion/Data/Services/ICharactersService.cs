using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndCompanion.Models;

namespace DndCompanion.Data.Services
{
    public interface ICharactersService
    {
        Task AddCharacterAsync(CharacterModel character, UserModel user);
    
        Task<IEnumerable<CharacterModel>> GetCharactersByUserAsync(UserModel user);

        Task<CharacterModel> GetCharacterByIdAsync(int id);

        Task DeleteCharacterAsync(int id);
    }
}