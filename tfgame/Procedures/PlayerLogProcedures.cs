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
    public class PlayerLogProcedures
    {
        public static IEnumerable<PlayerLog> GetAllPlayerLogs(int playerId)
        {
            IPlayerLogRepository playerLogs = new EFPlayerLogRepository();
            return playerLogs.PlayerLogs.Where(l => l.PlayerId == playerId).OrderBy(l => l.Timestamp);

        }

        public static void AddPlayerLog(int playerId, string message, bool isImportant)
        {

            IPlayerLogRepository playerLogRepo = new EFPlayerLogRepository();
            PlayerLog newlog = new PlayerLog();
            newlog.PlayerId = playerId;
            newlog.Message = message;
            newlog.Timestamp = DateTime.UtcNow;
            newlog.IsImportant = isImportant;

            IEnumerable<PlayerLog> ExistingLogs = playerLogRepo.PlayerLogs.Where(l => l.PlayerId == playerId);

            // delete oldest entry to keep log size from growing too large
            if (ExistingLogs.Count() >= PvPStatics.MaxLogMessagesPerPlayer)
            {
                IEnumerable<PlayerLog> reordered = ExistingLogs.OrderBy(l => l.Timestamp).ToList();
                PlayerLog deleteMe = reordered.First();
                playerLogRepo.DeletePlayerLog(deleteMe.Id);
            }

            playerLogRepo.SavePlayerLog(newlog);

            NoticeProcedures.PushNotice(playerId, newlog.Message);

        }

        public static void ClearPlayerLog(int playerId)
        {
            IPlayerLogRepository playerLogRepo = new EFPlayerLogRepository();
            IEnumerable<PlayerLog> myLogs = playerLogRepo.PlayerLogs.Where(l => l.PlayerId == playerId).ToList();

            foreach (PlayerLog log in myLogs)
            {
                playerLogRepo.DeletePlayerLog(log.Id);
            }
        }

        public static void DismissImportantLogs(int playerId)
        {
            IPlayerLogRepository playerLogRepo = new EFPlayerLogRepository();
            IEnumerable<PlayerLog> myLogs = playerLogRepo.PlayerLogs.Where(l => l.PlayerId == playerId && l.IsImportant == true).ToList();

            foreach (PlayerLog log in myLogs)
            {
                log.IsImportant = false;
               
            }

            foreach (PlayerLog log in myLogs)
            {
                playerLogRepo.SavePlayerLog(log);

            }
        }

        public static bool PlayerAlreadyHasMessage(int playerId, string message)
        {
            IPlayerLogRepository playerLogRepo = new EFPlayerLogRepository();
            if (playerLogRepo.PlayerLogs.FirstOrDefault(p => p.PlayerId == playerId && p.Message == message) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}