using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFItemTransferLogRepository : IItemTransferLogRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<ItemTransferLog> ItemTransferLogs
        {
            get { return context.ItemTransferLogs; }
        }

        public IQueryable<Player> Players
        {
            get { return context.Players; }
        }

        public void SaveItemTransferLog(ItemTransferLog ItemTransferLog)
        {
            if (ItemTransferLog.Id == 0)
            {
                context.ItemTransferLogs.Add(ItemTransferLog);
            }
            else
            {
                var editMe = context.ItemTransferLogs.Find(ItemTransferLog.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = ItemTransferLog.Name;
                    // dbEntry.Message = ItemTransferLog.Message;
                    // dbEntry.TimeStamp = ItemTransferLog.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteItemTransferLog(int id)
        {

            var dbEntry = context.ItemTransferLogs.Find(id);
            if (dbEntry != null)
            {
                context.ItemTransferLogs.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}