using System.Data.Entity.Core;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
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