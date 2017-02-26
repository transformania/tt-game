using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IBuffRepository
    {

        IQueryable<Buff> Buffs { get; }

        void SaveBuff(Buff Buff);

        void DeleteBuff(int BuffId);

    }
}