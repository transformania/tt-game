using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class LocationLog
    {
        public int Id { get; set; }
        public string dbLocationName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public int ConcealmentLevel { get; set; }
    }
}