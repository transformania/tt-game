using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.ViewModels;
using tfgame.Statics;

namespace tfgame.Procedures
{
    public class ItemTransferLogProcedures
    {
        public static List<ItemTransferLogViewModel> GetItemTransferLog(int iItem)
        {
            IItemTransferLogRepository ItemTransferLogRepo = new EFItemTransferLogRepository();
            IPlayerRepository PlayerRepo = new EFPlayerRepository();
            var query = from p in ItemTransferLogRepo.ItemTransferLogs
                        where p.ItemId == iItem
                        from o in ItemTransferLogRepo.Players
                        .Where(q => q.Id == p.OwnerId)
                        .DefaultIfEmpty()
                        select new ItemTransferLogViewModel
                        {
                            OwnerIP = o.IpAddress ?? "-1",
                            OwnerName = (o.FirstName == null || o.LastName == null ? "-1" : o.FirstName + " " + o.LastName),
                            ItemLog = new ItemTransferLog_VM
                            {
                                Id = p.Id,
                                ItemId = p.ItemId,
                                OwnerId = p.OwnerId,
                                Timestamp = p.Timestamp
                            }
                        };
            List<ItemTransferLogViewModel> output = query.OrderBy(p => p.ItemLog.Timestamp).ToList();

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