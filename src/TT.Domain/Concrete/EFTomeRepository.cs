using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFTomeRepository : ITomeRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Tome> Tomes
        {
            get { return context.Tomes; }
        }

        public void SaveTome(Tome Tome)
        {
            if (Tome.Id == 0)
            {
                context.Tomes.Add(Tome);
            }
            else
            {
                Tome editMe = context.Tomes.Find(Tome.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = ItemTransferLog.Name;
                    // dbEntry.Message = ItemTransferLog.Message;
                    // dbEntry.TimeStamp = ItemTransferLog.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteTome(int id)
        {

            Tome dbEntry = context.Tomes.Find(id);
            if (dbEntry != null)
            {
                context.Tomes.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}