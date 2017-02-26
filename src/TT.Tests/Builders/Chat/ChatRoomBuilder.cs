using System;
using TT.Domain.Chat.Entities;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Builders.Chat
{
    public class ChatRoomBuilder : Builder<ChatRoom, int>
    {
        public ChatRoomBuilder()
        {
            Instance = Create();
            With(x => x.Name, "Test_Room");
            With(x => x.Creator, new UserBuilder().BuildAndSave());
            With(x => x.CreatedAt, DateTime.UtcNow);
            With(x => x.Topic, "No sex in global!");
        }
    }
}