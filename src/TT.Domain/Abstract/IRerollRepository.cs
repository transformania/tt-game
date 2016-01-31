using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IRerollRepository
    {

        IQueryable<Reroll> Rerolls { get; }

        void SaveReroll(Reroll Reroll);

        void DeleteReroll(int RerollId);

    }
}