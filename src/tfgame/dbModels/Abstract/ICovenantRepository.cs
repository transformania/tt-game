using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface ICovenantRepository
    {

        IQueryable<Covenant> Covenants { get; }

        void SaveCovenant(Covenant Covenant);

        void DeleteCovenant(int CovenantId);

    }
}