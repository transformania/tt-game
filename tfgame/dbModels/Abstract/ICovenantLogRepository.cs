using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface ICovenantLogRepository
    {

        IQueryable<CovenantLog> CovenantLogs { get; }

        void SaveCovenantLog(CovenantLog CovenantLog);

        void DeleteCovenantLog(int CovenantLogId);

    }
}