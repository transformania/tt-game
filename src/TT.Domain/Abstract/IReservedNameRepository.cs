using System.Linq;
using tfgame.dbModels.Models;


namespace tfgame.dbModels.Abstract
{
    public interface IReservedNameRepository
    {
        IQueryable<ReservedName> ReservedNames { get; }

        void SaveReservedName(ReservedName ReservedName);

        void DeleteReservedName(int ReservedNameId);
    }
}