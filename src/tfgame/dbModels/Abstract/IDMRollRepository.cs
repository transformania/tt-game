using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IDMRollRepository
    {

        IQueryable<DMRoll> DMRolls { get; }

        void SaveDMRoll(DMRoll DMRoll);

        void DeleteDMRoll(int DMRollId);

    }
}