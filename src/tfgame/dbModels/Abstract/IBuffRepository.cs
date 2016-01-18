using System.Linq;

namespace tfgame.dbModels.Abstract
{
    public interface IBuffRepository
    {

        IQueryable<Buff> Buffs { get; }

        void SaveBuff(Buff Buff);

        void DeleteBuff(int BuffId);

    }
}