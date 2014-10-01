using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IBuffRepository
    {

        IQueryable<Buff> Buffs { get; }

        void SaveBuff(Buff Buff);

        void DeleteBuff(int BuffId);

    }
}