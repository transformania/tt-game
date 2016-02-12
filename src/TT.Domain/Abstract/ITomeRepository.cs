using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface ITomeRepository
    {

        IQueryable<Tome> Tomes { get; }

        void SaveTome(Tome Tome);

        void DeleteTome(int TomeId);

    }
}