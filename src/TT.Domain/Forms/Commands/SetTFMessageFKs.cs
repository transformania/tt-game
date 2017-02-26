using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;

namespace TT.Domain.Forms.Commands
{
    public class SetTFMessageFKs : DomainCommand
    {
        public int TFMessageId { get; set; }
        public string FormSource { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var tfMessage = ctx.AsQueryable<TFMessage>().SingleOrDefault(t => t.Id == TFMessageId);
                if (tfMessage == null)
                    throw new DomainException($"TFMessage with Id {TFMessageId} could not be found");

                var form = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.dbName == FormSource);

                if (form == null)
                {
                    throw new DomainException($"FormSource with name '{FormSource}' could not be found");
                }

                form.SetTFMessage(tfMessage);

                ctx.Update(form);
                ctx.Commit();

            };

            ExecuteInternal(context);

        }
    }
}
