using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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