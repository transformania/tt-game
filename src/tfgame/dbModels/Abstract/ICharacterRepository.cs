using System.Linq;
using tfgame.dbModels.Models;


namespace tfgame.dbModels.Abstract
{
    public interface ICharacterRepository
    {
        IQueryable<Character> Characters { get; }

        void SaveCharacter(Character Character);

        void DeleteCharacter(int CharacterId);
    }
}