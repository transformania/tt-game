using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFEffectContributionRepository : IEffectContributionRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<EffectContribution> EffectContributions
        {
            get { return context.EffectContributions; }
        }

        public void SaveEffectContribution(EffectContribution EffectContribution)
        {
            if (EffectContribution.Id == 0)
            {
                context.EffectContributions.Add(EffectContribution);
            }
            else
            {
                var editMe = context.EffectContributions.Find(EffectContribution.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = EffectContribution.Name;
                    // dbEntry.Message = EffectContribution.Message;
                    // dbEntry.TimeStamp = EffectContribution.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteEffectContribution(int id)
        {

            var dbEntry = context.EffectContributions.Find(id);
            if (dbEntry != null)
            {
                context.EffectContributions.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}