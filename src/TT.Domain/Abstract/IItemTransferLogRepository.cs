using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IItemTransferLogRepository
    {

        IQueryable<ItemTransferLog> ItemTransferLogs { get; }
        IQueryable<Player> Players { get; }

        void SaveItemTransferLog(ItemTransferLog ItemTransferLog);

        void DeleteItemTransferLog(int ItemTransferLogId);

    }
}