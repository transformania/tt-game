using System;
using TT.Domain.Entities.Chat;

namespace TT.Tests.Builders.Chat
{
    public class ChatRoomBuilder : Builder<ChatRoom, int>
    {
        public ChatRoomBuilder()
        {
            Instance = Create();
            With(x => x.Name, "Test Room");
            With(x => x.Creator, Guid.NewGuid().ToString());
            With(x => x.CreatedAt, DateTime.UtcNow);
        }
    }
}