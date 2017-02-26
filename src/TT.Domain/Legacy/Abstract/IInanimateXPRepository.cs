using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IInanimateXPRepository
    {

        IQueryable<InanimateXP> InanimateXPs { get; }

        void SaveInanimateXP(InanimateXP InanimateXP);

        void DeleteInanimateXP(int InanimateXPId);

    }
}