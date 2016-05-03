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

        public override int Execute(IDataContext context)
        {
            int result = 0;

            ContextQuery = ctx =>
            {
                var sender = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(t => t.Id == SenderId);
                if (sender == null)
                    throw new DomainException(string.Format("Player with Id {0} could not be found", SenderId));

                var receiver = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(t => t.Id == SenderId);
                if (sender == null)
                    throw new DomainException(string.Format("Player with Id {0} could not be found", SenderId));

                var message = Message.Create(sender, receiver);

                ctx.Add(message);
                ctx.Commit();

                result = message.Id;
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            // text not blank
        }
    }
}
