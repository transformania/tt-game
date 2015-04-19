using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IItemTransferLogRepository
    {

        IQueryable<ItemTransferLog> ItemTransferLogs { get; }

        void SaveItemTransferLog(ItemTransferLog ItemTransferLog);

        void DeleteItemTransferLog(int ItemTransferLogId);

    }
}