using System;

namespace TT.Domain.Entities.LocationLogs
{
    public class LocationLog : Entity<int>
    {
        public string dbLocationName { get; protected set; }
        public string Message { get; protected set; }
        public DateTime Timestamp { get; protected set; }
        public int ConcealmentLevel { get; protected set; }

        private LocationLog() { }

    }
}
