using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface ICovenantLogRepository
    {

        IQueryable<CovenantLog> CovenantLogs { get; }

        void SaveCovenantLog(CovenantLog CovenantLog);

        void DeleteCovenantLog(int CovenantLogId);

    }
}