using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFCovenantLogRepository : ICovenantLogRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<CovenantLog> CovenantLogs
        {
            get { return context.CovenantLogs; }
        }

        public void SaveCovenantLog(CovenantLog CovenantLog)
        {
            if (CovenantLog.Id == 0)
            {
                context.CovenantLogs.Add(CovenantLog);
            }
            else
            {
                CovenantLog editMe = context.CovenantLogs.Find(CovenantLog.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = CovenantLog.Name;
                    // dbEntry.Message = CovenantLog.Message;
                    // dbEntry.TimeStamp = CovenantLog.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteCovenantLog(int id)
        {

            CovenantLog dbEntry = context.CovenantLogs.Find(id);
            if (dbEntry != null)
            {
                context.CovenantLogs.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}