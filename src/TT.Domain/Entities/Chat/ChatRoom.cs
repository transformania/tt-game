using System;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Entities.Chat
{
    public class ChatRoom : Entity<int>
    {
        public string Name { get; protected set; }
        public User Creator { get; protected set; }
        public DateTime? CreatedAt { get; protected set; }

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