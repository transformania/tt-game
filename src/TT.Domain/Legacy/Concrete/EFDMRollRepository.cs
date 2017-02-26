using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFDMRollRepository : IDMRollRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<DMRoll> DMRolls
        {
            get { return context.DMRolls; }
        }

        public void SaveDMRoll(DMRoll DMRoll)
        {
            if (DMRoll.Id == 0)
            {
                context.DMRolls.Add(DMRoll);
            }
            else
            {
                DMRoll editMe = context.DMRolls.Find(DMRoll.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = DMRoll.Name;
                    // dbEntry.Message = DMRoll.Message;
                    // dbEntry.TimeStamp = DMRoll.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteDMRoll(int id)
        {

            DMRoll dbEntry = context.DMRolls.Find(id);
            if (dbEntry != null)
            {
                context.DMRolls.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}