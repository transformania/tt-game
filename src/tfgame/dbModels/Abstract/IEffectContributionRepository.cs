using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IEffectContributionRepository
    {

        IQueryable<EffectContribution> EffectContributions { get; }

        void SaveEffectContribution(EffectContribution EffectContribution);

        void DeleteEffectContribution(int EffectContributionId);

    }
}