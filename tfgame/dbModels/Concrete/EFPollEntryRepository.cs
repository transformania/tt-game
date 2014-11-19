using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFPollEntriesRepository : IPollEntryRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<PollEntry> PollEntries
        {
            get { return context.PollEntries; }
        }

        public void SavePollEntry(PollEntry PollEntries)
        {
            if (PollEntries.Id == 0)
            {
                context.PollEntries.Add(PollEntries);
            }
            else
            {
                PollEntry editMe = context.PollEntries.Find(PollEntries.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = PollEntries.Name;
                    // dbEntry.Message = PollEntries.Message;
                    // dbEntry.TimeStamp = PollEntries.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeletePollEntry(int id)
        {

            PollEntry dbEntry = context.PollEntries.Find(id);
            if (dbEntry != null)
            {
                context.PollEntries.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}