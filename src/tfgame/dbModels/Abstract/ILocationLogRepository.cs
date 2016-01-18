using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface ILocationLogRepository
    {

        IQueryable<LocationLog> LocationLogs { get; }

        void SaveLocationLog(LocationLog LocationLog);

        void DeleteLocationLog(int LocationLogId);

    }
}