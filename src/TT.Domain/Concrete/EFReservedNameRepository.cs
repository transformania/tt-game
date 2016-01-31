using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;


namespace TT.Domain.Concrete
{
    public class EFReservedNameRepository : IReservedNameRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<ReservedName> ReservedNames
        {
            get { return context.ReservedNames; }
        }

        public void SaveReservedName(ReservedName ReservedName)
        {
            if (ReservedName.Id == 0)
            {
                context.ReservedNames.Add(ReservedName);
            }
            else
            {
                ReservedName editMe = context.ReservedNames.Find(ReservedName.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = ReservedNames.Name;
                    // dbEntry.Message = ReservedNames.Message;
                    // dbEntry.TimeStamp = ReservedNames.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteReservedName(int id)
        {

            ReservedName dbEntry = context.ReservedNames.Find(id);
            if (dbEntry != null)
            {
                context.ReservedNames.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}