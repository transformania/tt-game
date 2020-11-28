using System;

namespace TT.Domain.Models
{
    public class PlayerLog
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsImportant { get; set; }
        public bool HideLog { get; set; }

    }
}