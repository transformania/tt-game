using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFItemTransferLogRepository : IItemTransferLogRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<ItemTransferLog> ItemTransferLogs
        {
            get { return context.ItemTransferLogs; }
        }

        public void SaveItemTransferLog(ItemTransferLog ItemTransferLog)
        {
            if (ItemTransferLog.Id == 0)
            {
                context.ItemTransferLogs.Add(ItemTransferLog);
            }
            else
            {
                ItemTransferLog editMe = context.ItemTransferLogs.Find(ItemTransferLog.Id);
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

            ItemTransferLog dbEntry = context.ItemTransferLogs.Find(id);
            if (dbEntry != null)
            {
                context.ItemTransferLogs.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}