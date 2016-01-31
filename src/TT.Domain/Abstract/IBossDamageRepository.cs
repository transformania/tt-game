using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IBossDamageRepository
    {

        IQueryable<BossDamage> BossDamages { get; }

        void SaveBossDamage(BossDamage BossDamage);

        void DeleteBossDamage(int BossDamageId);

    }
}