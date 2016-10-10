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

        public static LocationLog Create(String dbLocationName, String message, int concealmentLevel)
        {
            return new LocationLog
            {
                dbLocationName = dbLocationName,
                Message = message,
                ConcealmentLevel = concealmentLevel,
                Timestamp = DateTime.UtcNow
            };
        }

    }
}
