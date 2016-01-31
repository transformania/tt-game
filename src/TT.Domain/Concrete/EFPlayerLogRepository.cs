using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFPlayerLogRepository : IPlayerLogRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<PlayerLog> PlayerLogs
        {
            get { return context.PlayerLogs; }
        }

        public void SavePlayerLog(PlayerLog PlayerLog)
        {
            if (PlayerLog.Id == 0)
            {
                context.PlayerLogs.Add(PlayerLog);
            }
            else
            {
                PlayerLog editMe = context.PlayerLogs.Find(PlayerLog.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = PlayerLog.Name;
                    // dbEntry.Message = PlayerLog.Message;
                    // dbEntry.TimeStamp = PlayerLog.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeletePlayerLog(int id)
        {

            PlayerLog dbEntry = context.PlayerLogs.Find(id);
            if (dbEntry != null)
            {
                context.PlayerLogs.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}