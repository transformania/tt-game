using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IItemTransferLogRepository
    {

        IQueryable<ItemTransferLog> ItemTransferLogs { get; }
        IQueryable<Player> Players { get; }

        void SaveItemTransferLog(ItemTransferLog ItemTransferLog);

        void DeleteItemTransferLog(int ItemTransferLogId);

    }
}