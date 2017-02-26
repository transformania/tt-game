using System;

namespace TT.Domain.Players.DTOs
{
    public class PlayerLogDetail
    {
        public int Id { get; set; }
        public PlayerDetail Owner { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsImportant { get; set; }
    }
}
