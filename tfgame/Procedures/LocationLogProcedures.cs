using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;

namespace tfgame.Procedures
{
    public class LocationLogProcedures
    {

        

        public static IEnumerable<LocationLog> GetLocationLogsAtLocation(string dbLocationName)
        {
            ILocationLogRepository LocationLogRepo = new EFLocationLogRepository();
            IEnumerable<LocationLog> output = LocationLogRepo.LocationLogs.Where(p => p.dbLocationName == dbLocationName).OrderBy(l => l.Timestamp).ToList();

            return output;
        }

        public static void AddLocationLog(string dbLocationName, string message)
        {
            ILocationLogRepository LocationLogRepo = new EFLocationLogRepository();
            LocationLog newlog = new LocationLog();
            newlog.dbLocationName = dbLocationName;
            newlog.Message = message;
            newlog.Timestamp = DateTime.UtcNow;

            IEnumerable<LocationLog> ExistingLogs = LocationLogRepo.LocationLogs.Where(l => l.dbLocationName == dbLocationName);


            // delete oldest entry to keep log size from growing too large
            if (ExistingLogs.Count() >= PvPStatics.MaxLogMessagesPerLocation)
            {
                IEnumerable<LocationLog> reordered = ExistingLogs.OrderByDescending(l => l.Timestamp).Skip(PvPStatics.MaxLogMessagesPerLocation).ToList();

                foreach (LocationLog l in reordered)
                {
                    try
                    {
                        LocationLogRepo.DeleteLocationLog(l.Id);
                    }
                    catch
                    {
                        // catch errors in case of concurrency problems... players moving during updates.
                    }
                }
            }
           
            LocationLogRepo.SaveLocationLog(newlog);
        }

    }
}