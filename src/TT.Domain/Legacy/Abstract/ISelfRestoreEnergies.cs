using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface ISelfRestoreEnergies
    {

        IQueryable<Models.SelfRestoreEnergies> SelfRestoreEnergies { get; }

        void DeleteSelfRestoreEnergies(int SelfRestoreEnergiesId);

    }
}