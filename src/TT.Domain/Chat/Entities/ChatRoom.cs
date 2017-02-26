using System;
using TT.Domain.Entities;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Chat.Entities
{
    public class ChatRoom : Entity<int>
    {
        public string Name { get; protected set; }
        public User Creator { get; protected set; }
        public DateTime? CreatedAt { get; protected set; }
        public string Topic { get; protected set; }

        private ChatRoom() { }

        public static ChatRoom Create(User creator, string roomName)
        {
            return new ChatRoom
            {
                Name = roomName,
                Creator = creator,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}