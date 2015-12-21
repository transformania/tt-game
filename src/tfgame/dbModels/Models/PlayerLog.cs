using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class PlayerLog
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsImportant { get; set; }
    }
}