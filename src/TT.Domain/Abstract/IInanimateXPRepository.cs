using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IInanimateXPRepository
    {

        IQueryable<InanimateXP> InanimateXPs { get; }

        void SaveInanimateXP(InanimateXP InanimateXP);

        void DeleteInanimateXP(int InanimateXPId);

    }
}