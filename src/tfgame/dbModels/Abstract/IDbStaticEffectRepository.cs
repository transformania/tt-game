using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IDbStaticEffectRepository
    {

        IQueryable<DbStaticEffect> DbStaticEffects { get; }

        void SaveDbStaticEffect(DbStaticEffect DbStaticEffect);

        void DeleteDbStaticEffect(int DbStaticEffectId);

    }
}