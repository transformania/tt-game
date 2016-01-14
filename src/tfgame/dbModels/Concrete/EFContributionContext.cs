using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFContributionRepository : IContributionRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Contribution> Contributions
        {
            get { return context.Contributions; }
        }

        public void SaveContribution(Contribution Contribution)
        {
            if (Contribution.Id == 0)
            {
                context.Contributions.Add(Contribution);
            }
            else
            {
                Contribution editMe = context.Contributions.Find(Contribution.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Contribution.Name;
                    // dbEntry.Message = Contribution.Message;
                    // dbEntry.TimeStamp = Contribution.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteContribution(int id)
        {

            Contribution dbEntry = context.Contributions.Find(id);
            if (dbEntry != null)
            {
                context.Contributions.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}