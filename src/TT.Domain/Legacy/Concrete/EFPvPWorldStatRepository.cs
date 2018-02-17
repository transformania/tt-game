using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFPvPWorldStatRepository : IPvPWorldStatRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<PvPWorldStat> PvPWorldStats
        {
            get { return context.PvPWorldStats; }
        }

        public void SavePvPWorldStat(PvPWorldStat PvPWorldStat)
        {
            if (PvPWorldStat.Id == 0)
            {
                context.PvPWorldStats.Add(PvPWorldStat);
            }
            else
            {
                var editMe = context.PvPWorldStats.Find(PvPWorldStat.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = PvPWorldStat.Name;
                    // dbEntry.Message = PvPWorldStat.Message;
                    // dbEntry.TimeStamp = PvPWorldStat.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeletePvPWorldStat(int id)
        {

            var dbEntry = context.PvPWorldStats.Find(id);
            if (dbEntry != null)
            {
                context.PvPWorldStats.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void ReloadPvPWorldStat(PvPWorldStat PvPWorldStat)
        {
            context.Entry(PvPWorldStat).Reload();
        }
    }
}