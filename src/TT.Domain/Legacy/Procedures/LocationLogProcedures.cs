using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Procedures
{
    public class LocationLogProcedures
    {

        public static void AddLocationLog(string dbLocationName, string message)
        {
            // -999 is a low enough concealment that anyone should see it
            AddLocationLog(dbLocationName, message, -999);
        }

        public static void AddLocationLog(string dbLocationName, string message, int concealmentLevel)
        {
            ILocationLogRepository LocationLogRepo = new EFLocationLogRepository();
            LocationLog newlog = new LocationLog();
            newlog.dbLocationName = dbLocationName;
            newlog.Message = message;
            newlog.Timestamp = DateTime.UtcNow;
            newlog.ConcealmentLevel = concealmentLevel;
            LocationLogRepo.SaveLocationLog(newlog);
        }

    }
}