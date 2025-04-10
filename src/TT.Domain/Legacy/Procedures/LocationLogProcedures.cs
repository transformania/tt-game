using System;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Procedures
{
    public class LocationLogProcedures
    {
        /*
         * With !1448 being introduced, raw html for the location logs was switched from Raw() to Encode().
         * This causes certain instances of meditating, searching, bold text, etc to not be properly displayed.
         * A nullable log type should help to resolve this issue, with the following values:
         * 
         * 0 = LOG_TYPE_BOLD
         * 1 = LOG_TYPE_BAD
         * 2 = LOG_TYPE_GOOD
         * 3 = LOG_TYPE_ATTACK
         * 4 = LOG_TYPE_CLEANSE
         * 5 = LOG_TYPE_MEDITATE
         * 6 = LOG_TYPE_SEARCH
         */
        public static void AddLocationLog(string dbLocationName, string message, int? logType = null)
        {
            // -999 is a low enough concealment that anyone should see it
            AddLocationLog(dbLocationName, message, -999, logType);
        }

        public static void AddLocationLog(string dbLocationName, string message, int concealmentLevel, int? logType)
        {
            ILocationLogRepository LocationLogRepo = new EFLocationLogRepository();
            var newlog = new LocationLog();
            newlog.dbLocationName = dbLocationName;
            newlog.Message = message;
            newlog.Timestamp = DateTime.UtcNow;
            newlog.ConcealmentLevel = concealmentLevel;
            newlog.LogType = logType;
            LocationLogRepo.SaveLocationLog(newlog);
        }

    }
}