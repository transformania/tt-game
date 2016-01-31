using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface ICovenantRepository
    {

        IQueryable<Covenant> Covenants { get; }

        void SaveCovenant(Covenant Covenant);

        void DeleteCovenant(int CovenantId);

    }
}