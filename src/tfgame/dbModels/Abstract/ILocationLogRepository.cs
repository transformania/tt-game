using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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