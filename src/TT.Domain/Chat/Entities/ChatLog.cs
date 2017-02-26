using System;
using TT.Domain.Entities;

namespace TT.Domain.Chat.Entities
{
    public class ChatLog : Entity<int>
    {
        public string Message { get; protected set; }
        public DateTime Timestamp { get; protected set; }
        public string Room { get; protected set; }

        public string UserId { get; protected set; }
        public string Name { get; protected set; }
        public string PortraitUrl { get; protected set; }
        public string Color { get; protected set; }

        private ChatLog() { }

        public static ChatLog Create(string message, string room, string name, string userId, string portraitUrl, string color)
        {
            return new ChatLog
            {
                Message = message,
                Room = room,
                Timestamp = DateTime.UtcNow,
                Name = name,
                UserId = userId,
                PortraitUrl = portraitUrl,
                Color = color
            };
        }
    }
}
