
using System;
using TT.Domain.Entities;

namespace TT.Domain.Players.Entities
{
    public class PlayerLog : Entity<int>
    {

        public Player Owner { get; protected set; }
        public string Message { get; protected set; }
        public DateTime Timestamp { get; protected set; }
        public bool IsImportant { get; protected set; }

        private PlayerLog()
        {
        }

        public static PlayerLog Create(Player owner, string message, DateTime timestamp, bool isImportant)
        {
            return new PlayerLog
            {
                Owner = owner,
                Message = message,
                Timestamp = timestamp,
                IsImportant = isImportant
            };
        }
    }
}
