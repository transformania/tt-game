using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFPlayerExtraRepository : IPlayerExtraRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<PlayerExtra> PlayerExtras
        {
            get { return context.PlayerExtras; }
        }

        public void SavePlayerExtra(PlayerExtra PlayerExtra)
        {
            if (PlayerExtra.Id == 0)
            {
                context.PlayerExtras.Add(PlayerExtra);
            }
            else
            {
                var editMe = context.PlayerExtras.Find(PlayerExtra.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = PlayerExtra.Name;
                    // dbEntry.PlayerExtra = PlayerExtra.PlayerExtra;
                    // dbEntry.TimeStamp = PlayerExtra.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeletePlayerExtra(int id)
        {

            var dbEntry = context.PlayerExtras.Find(id);
            if (dbEntry != null)
            {
                context.PlayerExtras.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}