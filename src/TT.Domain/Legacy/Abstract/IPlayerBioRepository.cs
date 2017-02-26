using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IPlayerBioRepository
    {

        IQueryable<PlayerBio> PlayerBios { get; }

        void SavePlayerBio(PlayerBio PlayerBio);

        void DeletePlayerBio(int PlayerBioId);

    }
}