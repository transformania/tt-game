using Highway.Data;
using TT.Domain.Chat.Entities;

namespace TT.Domain.Chat.Commands
{
    public class CreateChatLog : DomainCommand
    {
        public string Message { get; set; }
        public string Room { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string PortraitUrl { get; set; }
        public string Color { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
               
                var log = ChatLog.Create(Message, Room, Name, UserId, PortraitUrl, Color);

                ctx.Add(log);
                ctx.Commit();

            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
           
        }
    }
}