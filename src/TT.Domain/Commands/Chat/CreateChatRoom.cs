using System.Linq;
using System.Text.RegularExpressions;
using Highway.Data;
using TT.Domain.Entities.Chat;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Commands.Chat
{
    public class CreateChatRoom : DomainCommand<ChatRoom>
    {
        public string RoomName { get; private set; }
        public string CreatorId { get; private set; }

        public CreateChatRoom(string roomName, string creatorId)
        {
            RoomName = roomName;
            CreatorId = creatorId;
        }

        public override ChatRoom Execute(IDataContext context)
        {
            Validate();

            ChatRoom room = null;

            ContextQuery = ctx =>
            {
                if (ctx.AsQueryable<ChatRoom>().Any(cr => cr.Name == RoomName))
                    throw new DomainException("Chat room '{0}' already exists", RoomName);

                var creator = ctx.AsQueryable<User>().Single(u => u.Id == CreatorId);

                room = ChatRoom.Create(creator, RoomName);

                ctx.Add(room);
                ctx.Commit();
            };

            ExecuteInternal(context);

            return room;
        }

        private void Validate()
        {
            var regex = new Regex("^[a-zA-Z0-9_-]*$");
            if (!regex.IsMatch(RoomName))
                throw new DomainException("Chat room '{0}' contains unsupported characters, only alphanumeric names with _ or - are allowed",RoomName);
        }
    }
}