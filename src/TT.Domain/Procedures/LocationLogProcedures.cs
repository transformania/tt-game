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

        
        // return all location logs regardless of concealment level
        public static IEnumerable<LocationLog> GetLocationLogsAtLocation(string dbLocationName)
        {
            ILocationLogRepository LocationLogRepo = new EFLocationLogRepository();
            IEnumerable<LocationLog> output = LocationLogRepo.LocationLogs.Where(p => p.dbLocationName == dbLocationName).OrderBy(l => l.Timestamp).ToList();

            return output;
        }

        // return all location logs where concealment level is below a given amount
        public static IEnumerable<LocationLog> GetLocationLogsAtLocation(string dbLocationName, int highestConcealmentLevel)
        {
            ILocationLogRepository LocationLogRepo = new EFLocationLogRepository();
            IEnumerable<LocationLog> output = LocationLogRepo.LocationLogs.Where(p => p.dbLocationName == dbLocationName && p.ConcealmentLevel <= highestConcealmentLevel).OrderBy(l => l.Timestamp).ToList();

            return output;
        }

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

        public static void Shout(Player player, string message)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.MembershipId == player.MembershipId);
            dbPlayer.ShoutsRemaining--;
            playerRepo.SavePlayer(dbPlayer);

            message = "<span class='playerShoutNotification'>" + player.GetFullName() + " shouted <b>\"" + message + "\"</b> here.</span>";

            AddLocationLog(player.dbLocationName, message);
        }

    }
}