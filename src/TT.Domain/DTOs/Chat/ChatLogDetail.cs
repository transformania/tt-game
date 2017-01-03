using System;

namespace TT.Domain.DTOs.Chat
{
    public class ChatLogDetail
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string Room { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string PortraitUrl { get; set; }
        public string Color { get; set; }

    }
}
