using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Commands.Messages
{
    public class CreateMessage : DomainCommand<int>
    {
        public string Text { get; set; }
        public int ReceiverId { get; set; }
        public int SenderId { get; set; }
        public bool DoNotRecycleMe { get; set; }
        public int ConversationId { get; set; }
        public bool IsRead { get; set; }
        public int ReadStatus { get; set; }

        public override int Execute(IDataContext context)
        {
            int result = 0;

            ContextQuery = ctx =>
            {
                var sender = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(t => t.Id == SenderId);
                if (sender == null)
                    throw new DomainException(string.Format("Sending player with Id {0} could not be found", SenderId));

                var receiver = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(t => t.Id == ReceiverId);
                if (receiver == null)
                    throw new DomainException(string.Format("Receiving player with Id {0} could not be found", ReceiverId));

                var message = Message.Create(sender, receiver, this);

                ctx.Add(message);
                ctx.Commit();

                result = message.Id;
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Text))
                throw new DomainException("Text must not be empty or null");
        }
    }
}
