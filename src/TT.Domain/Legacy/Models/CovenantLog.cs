using System;

namespace TT.Domain.Models
{
    public class CovenantLog
    {
        public int Id { get; set; }
        public int CovenantId { get; set;}
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsImportant { get; set; }
    }
}