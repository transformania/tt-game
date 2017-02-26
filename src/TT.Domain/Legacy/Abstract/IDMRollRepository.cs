using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IDMRollRepository
    {

        IQueryable<DMRoll> DMRolls { get; }

        void SaveDMRoll(DMRoll DMRoll);

        void DeleteDMRoll(int DMRollId);

    }
}