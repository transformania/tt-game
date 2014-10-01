using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IEffectRepository
    {

        IQueryable<Effect> Effects { get; }

        void SaveEffect(Effect Effect);

        void DeleteEffect(int EffectId);

    }
}