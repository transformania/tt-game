using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IDuelRepository
    {

        IQueryable<Duel> Duels { get; }

        void SaveDuel(Duel Duel);

        void DeleteDuel(int DuelId);

    }
}