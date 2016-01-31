using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IEffectContributionRepository
    {

        IQueryable<EffectContribution> EffectContributions { get; }

        void SaveEffectContribution(EffectContribution EffectContribution);

        void DeleteEffectContribution(int EffectContributionId);

    }
}