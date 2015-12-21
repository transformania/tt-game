using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
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