using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IPlayerLogRepository
    {

        IQueryable<PlayerLog> PlayerLogs { get; }

        void SavePlayerLog(PlayerLog PlayerLog);

        void DeletePlayerLog(int PlayerLogId);

    }
}