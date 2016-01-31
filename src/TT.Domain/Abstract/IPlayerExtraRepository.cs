using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IPlayerExtraRepository
    {

        IQueryable<PlayerExtra> PlayerExtras { get; }

        void SavePlayerExtra(PlayerExtra PlayerExtra);

        void DeletePlayerExtra(int PlayerExtraId);

    }
}