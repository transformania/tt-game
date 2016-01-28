using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IServerLogRepository
    {

        IQueryable<ServerLog> ServerLogs { get; }

        void SaveServerLog(ServerLog ServerLog);

        void DeleteServerLog(int ServerLogId);

    }
}