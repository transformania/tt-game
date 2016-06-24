using System;

namespace TT.Domain.DTOs.Players
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
