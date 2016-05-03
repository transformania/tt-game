using System;
using TT.Domain.Entities.Messages;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Item;

namespace TT.Tests.Builders.Messages
{
    public class MessageBuilder : Builder<Message, int>
    {
        public MessageBuilder()
        {
            Instance = Create();
            With(m => m.Id, 915);
            With(u => u.Sender, new PlayerBuilder()
                .With(p => p.User, new UserBuilder().With(u => u.Id, "guid").BuildAndSave())
                .With(p => p.Id, 50)
                .With(p => p.FirstName, "Sam")
                .With(p => p.LastName, "Houston")
                .BuildAndSave());
            With(u => u.Receiver, new PlayerBuilder()
                .With(p => p.User, new UserBuilder().With(u => u.Id, "guid2").BuildAndSave())
                .With(p => p.Id, 51)
                .With(p => p.FirstName, "Lora")
                .With(p => p.LastName, "Teetoo")
                .BuildAndSave());
            With(m => m.Timestamp, DateTime.UtcNow);
        }
    
    }
}
