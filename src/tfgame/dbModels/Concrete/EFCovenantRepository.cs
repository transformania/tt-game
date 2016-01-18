using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFCovenantRepository : ICovenantRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Covenant> Covenants
        {
            get { return context.Covenants; }
        }

        public void SaveCovenant(Covenant Covenant)
        {
            if (Covenant.Id == 0)
            {
                context.Covenants.Add(Covenant);
            }
            else
            {
                Covenant editMe = context.Covenants.Find(Covenant.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Covenant.Name;
                    // dbEntry.Message = Covenant.Message;
                    // dbEntry.TimeStamp = Covenant.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteCovenant(int id)
        {

            Covenant dbEntry = context.Covenants.Find(id);
            if (dbEntry != null)
            {
                context.Covenants.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}