using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Chat;

namespace TT.Domain.Commands.Chat
{
    public class CreateChatRoom : Highway.Data.Command
    {
        public string RoomName { get; private set; }
        public string CreatorId { get; private set; }

        public CreateChatRoom(string roomName, string creatorId)
        {
            RoomName = roomName;
            CreatorId = creatorId;
        }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                if (ctx.AsQueryable<ChatRoom>().Any(cr => cr.Name == RoomName))
                    throw new DomainException(string.Format("Chat room '{0}' already exists", RoomName));

                ctx.Add(ChatRoom.Create(this));
                ctx.Commit();
            };

            base.Execute(context);
        }
    }
}