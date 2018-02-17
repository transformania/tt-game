using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFBlacklistEntryRepository : IBlacklistEntryRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<BlacklistEntry> BlacklistEntries
        {
            get { return context.BlacklistEntries; }
        }

        public void SaveBlacklistEntry(BlacklistEntry BlacklistEntry)
        {
            if (BlacklistEntry.Id == 0)
            {
                context.BlacklistEntries.Add(BlacklistEntry);
            }
            else
            {
                var editMe = context.BlacklistEntries.Find(BlacklistEntry.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = BlacklistEntry.Name;
                    // dbEntry.Message = BlacklistEntry.Message;
                    // dbEntry.TimeStamp = BlacklistEntry.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteBlacklistEntry(int id)
        {

            var dbEntry = context.BlacklistEntries.Find(id);
            if (dbEntry != null)
            {
                context.BlacklistEntries.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}