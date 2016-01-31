using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IPlayerLogRepository
    {

        IQueryable<PlayerLog> PlayerLogs { get; }

        void SavePlayerLog(PlayerLog PlayerLog);

        void DeletePlayerLog(int PlayerLogId);

    }
}