using System;

namespace TT.Domain.World.DTOs
{
    public class LocationLogDetail
    {
        public int Id { get; set; }
        public string dbLocationName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public int ConcealmentLevel { get; set; }
    }
}
