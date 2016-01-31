using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface ILocationLogRepository
    {

        IQueryable<LocationLog> LocationLogs { get; }

        void SaveLocationLog(LocationLog LocationLog);

        void DeleteLocationLog(int LocationLogId);

    }
}