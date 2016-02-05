using System;
using TT.Domain.Commands.Chat;

namespace TT.Domain.Entities.Chat
{
    public class ChatRoom : Entity<int>
    {
        public string Name { get; protected set; }
        public string Creator { get; protected set; }
        public DateTime? CreatedAt { get; protected set; }

        private ChatRoom() { }

        public static ChatRoom Create(CreateChatRoom cmd)
        {
            return new ChatRoom
            {
                Name = cmd.RoomName,
                Creator = cmd.CreatorId,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}