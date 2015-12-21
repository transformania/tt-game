using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFDuelRepository : IDuelRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Duel> Duels
        {
            get { return context.Duels; }
        }

        public void SaveDuel(Duel Duel)
        {
            if (Duel.Id == 0)
            {
                context.Duels.Add(Duel);
            }
            else
            {
                Duel editMe = context.Duels.Find(Duel.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Duel.Name;
                    // dbEntry.Message = Duel.Message;
                    // dbEntry.TimeStamp = Duel.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.Duel);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteDuel(int id)
        {

            Duel dbEntry = context.Duels.Find(id);
            if (dbEntry != null)
            {
                context.Duels.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}