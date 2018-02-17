using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
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
                var editMe = context.Donators.Find(Donator.Id);
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

            var dbEntry = context.Donators.Find(id);
            if (dbEntry != null)
            {
                context.Donators.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}