using System.Linq;
using TT.Domain.Models;


namespace TT.Domain.Abstract
{
    public interface ICharacterRepository
    {
        IQueryable<Character> Characters { get; }

        void SaveCharacter(Character Character);

        void DeleteCharacter(int CharacterId);
    }
}