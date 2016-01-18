using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IPlayerBioRepository
    {

        IQueryable<PlayerBio> PlayerBios { get; }

        void SavePlayerBio(PlayerBio PlayerBio);

        void DeletePlayerBio(int PlayerBioId);

    }
}