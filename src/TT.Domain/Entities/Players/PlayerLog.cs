
using System;

namespace TT.Domain.Entities.Players
{
    public class PlayerLog : Entity<int>
    {
      
        public Player Owner { get; protected set; }
        public string Message { get; protected set; }
        public DateTime Timestamp { get; protected set; }
        public bool IsImportant { get; protected set; }

        private PlayerLog() { }
    }
}
