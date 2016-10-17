using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Forms;

namespace TT.Domain.Commands.Forms
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
                    throw new DomainException(string.Format("TFMessage with Id {0} could not be found", TFMessageId));

                var form = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.dbName == FormSource);

                if (form == null)
                {
                    throw new DomainException(string.Format("FormSource with name '{0}' could not be found", FormSource));
                }

                form.SetTFMessage(tfMessage);

                ctx.Update(form);
                ctx.Commit();

            };

            ExecuteInternal(context);

        }
    }
}
