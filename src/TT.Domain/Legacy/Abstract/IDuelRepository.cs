using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IDuelRepository
    {

        IQueryable<Duel> Duels { get; }

        void SaveDuel(Duel Duel);

        void DeleteDuel(int DuelId);

    }
}