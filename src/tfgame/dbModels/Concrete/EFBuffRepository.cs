using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFBuffRepository : IBuffRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Buff> Buffs
        {
            get { return context.Buffs; }
        }

        public void SaveBuff(Buff Buff)
        {
            if (Buff.Id == 0)
            {
                context.Buffs.Add(Buff);
            }
            else
            {
                Buff editMe = context.Buffs.Find(Buff.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Buff.Name;
                    // dbEntry.Message = Buff.Message;
                    // dbEntry.TimeStamp = Buff.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteBuff(int id)
        {

            Buff dbEntry = context.Buffs.Find(id);
            if (dbEntry != null)
            {
                context.Buffs.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}