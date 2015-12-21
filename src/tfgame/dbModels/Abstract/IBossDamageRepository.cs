using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IBossDamageRepository
    {

        IQueryable<BossDamage> BossDamages { get; }

        void SaveBossDamage(BossDamage BossDamage);

        void DeleteBossDamage(int BossDamageId);

    }
}