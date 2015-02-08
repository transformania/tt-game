using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class BookReading
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string BookDbName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}