using System.Linq;
using TT.Domain.Models;


namespace TT.Domain.Abstract
{
    public interface IReservedNameRepository
    {
        IQueryable<ReservedName> ReservedNames { get; }

        void SaveReservedName(ReservedName ReservedName);

        void DeleteReservedName(int ReservedNameId);
    }
}