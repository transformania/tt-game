using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFBossDamageRepository : IBossDamageRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<BossDamage> BossDamages
        {
            get { return context.BossDamages; }
        }

        public void SaveBossDamage(BossDamage BossDamage)
        {
            if (BossDamage.Id == 0)
            {
                context.BossDamages.Add(BossDamage);
            }
            else
            {
                BossDamage editMe = context.BossDamages.Find(BossDamage.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = BossDamage.Name;
                    // dbEntry.BossDamage = BossDamage.BossDamage;
                    // dbEntry.TimeStamp = BossDamage.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteBossDamage(int id)
        {

            BossDamage dbEntry = context.BossDamages.Find(id);
            if (dbEntry != null)
            {
                context.BossDamages.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}