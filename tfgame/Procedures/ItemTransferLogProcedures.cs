using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;

namespace tfgame.Procedures
{
    public class ItemTransferLogProcedures
    {
        public static IEnumerable<ItemTransferLog> GetItemTransferLog(int iItem)
        {
            IItemTransferLogRepository ItemTransferLogRepo = new EFItemTransferLogRepository();
            IEnumerable<ItemTransferLog> output = ItemTransferLogRepo.ItemTransferLogs.Where(p => p.ItemId == iItem).OrderBy(l => l.Timestamp).ToList();
            return output;
        }

        public static void AddItemTransferLog(int iItem, int iOwner)
        {
            IItemTransferLogRepository ItemTransferLogRepo = new EFItemTransferLogRepository();
            ItemTransferLog newlog = new ItemTransferLog();

            newlog.ItemId = iItem;
            newlog.OwnerId = iOwner;
            newlog.Timestamp = DateTime.UtcNow;
           
            ItemTransferLogRepo.SaveItemTransferLog(newlog);
        }
    }
}