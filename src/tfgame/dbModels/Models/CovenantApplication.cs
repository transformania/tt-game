using System;

namespace tfgame.dbModels.Models
{
    public class CovenantApplication
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int CovenantId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}