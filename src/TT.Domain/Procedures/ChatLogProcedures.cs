using System;
using System.Collections.Generic;
using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures
{
    public static class ChatLogProcedures
    {
        public static void WriteLogToDatabase(string room, string name, string message)
        {
            IChatLogRepository repo = new EFChatLogRepository();
            ChatLog newlog = new ChatLog
            {
                Message = name + ": " + message,
                Timestamp = DateTime.UtcNow,
                Room = room,
            };
            repo.SaveChatLog(newlog);
        }

        public static IEnumerable<ChatLog> GetChatLogs(string room, string filter)
        {
            IChatLogRepository repo = new EFChatLogRepository();

            DateTime cutoff;

            if (filter == "lasth")
            {
                cutoff = DateTime.UtcNow.AddHours(-1);
            }
            else if (filter == "last4h")
            {
                cutoff = DateTime.UtcNow.AddHours(-4);
            }
            else if (filter == "last12h")
            {
                cutoff = DateTime.UtcNow.AddHours(-12);
            }
            else if (filter == "last24h")
            {
                cutoff = DateTime.UtcNow.AddDays(-1);
            }
            else if (filter == "last48h")
            {
                cutoff = DateTime.UtcNow.AddDays(-2);
            }
            else if (filter == "last72h")
            {
                cutoff = DateTime.UtcNow.AddDays(-3);
            }
            else
            {
                cutoff = DateTime.UtcNow.AddHours(-1);
            }

            IEnumerable<ChatLog> output = repo.ChatLogs.Where(c => c.Room == room && c.Timestamp >= cutoff);
            return output;
        }
    }


}