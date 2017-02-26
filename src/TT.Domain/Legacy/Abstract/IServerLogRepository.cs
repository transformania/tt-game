using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IServerLogRepository
    {

        IQueryable<ServerLog> ServerLogs { get; }

        void SaveServerLog(ServerLog ServerLog);

        void DeleteServerLog(int ServerLogId);

    }
}