﻿using System;
using TT.Domain.Entities.Chat;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Builders.Chat
{
    public class ChatRoomBuilder : Builder<ChatRoom, int>
    {
        public ChatRoomBuilder()
        {
            Instance = Create();
            With(x => x.Name, "Test Room");
            With(x => x.Creator, new UserBuilder().BuildAndSave());
            With(x => x.CreatedAt, DateTime.UtcNow);
        }
    }
}