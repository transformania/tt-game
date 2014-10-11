using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFDonatorRepository : IDonatorRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Donator> Donators
        {
            get { return context.Donators; }
        }

        public void SaveDonator(Donator Donator)
        {
            if (Donator.Id == 0)
            {
                context.Donators.Add(Donator);
            }
            else
            {
                Donator editMe = context.Donators.Find(Donator.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Donator.Name;
                    // dbEntry.Message = Donator.Message;
                    // dbEntry.TimeStamp = Donator.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteDonator(int id)
        {

            Donator dbEntry = context.Donators.Find(id);
            if (dbEntry != null)
            {
                context.Donators.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}