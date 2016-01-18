using System;

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