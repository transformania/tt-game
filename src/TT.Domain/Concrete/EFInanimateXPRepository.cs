using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFInanimateXPRepository : IInanimateXPRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<InanimateXP> InanimateXPs
        {
            get { return context.InanimateXPs; }
        }

        public void SaveInanimateXP(InanimateXP InanimateXP)
        {
            if (InanimateXP.Id == 0)
            {
                context.InanimateXPs.Add(InanimateXP);
            }
            else
            {
                InanimateXP editMe = context.InanimateXPs.Find(InanimateXP.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = InanimateXP.Name;
                    // dbEntry.Message = InanimateXP.Message;
                    // dbEntry.TimeStamp = InanimateXP.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteInanimateXP(int id)
        {

            InanimateXP dbEntry = context.InanimateXPs.Find(id);
            if (dbEntry != null)
            {
                context.InanimateXPs.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}