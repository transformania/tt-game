using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IRerollRepository
    {

        IQueryable<Reroll> Rerolls { get; }

        void SaveReroll(Reroll Reroll);

        void DeleteReroll(int RerollId);

    }
}