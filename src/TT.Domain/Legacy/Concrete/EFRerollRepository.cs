using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFRerollRepository : IRerollRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Reroll> Rerolls
        {
            get { return context.Rerolls; }
        }

        public void SaveReroll(Reroll Reroll)
        {
            if (Reroll.Id == 0)
            {
                context.Rerolls.Add(Reroll);
            }
            else
            {
                var editMe = context.Rerolls.Find(Reroll.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = ItemTransferLog.Name;
                    // dbEntry.Message = ItemTransferLog.Message;
                    // dbEntry.TimeStamp = ItemTransferLog.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteReroll(int id)
        {

            var dbEntry = context.Rerolls.Find(id);
            if (dbEntry != null)
            {
                context.Rerolls.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}