using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Items.Entities;

namespace TT.Domain.Forms.Commands
{
    public class SetFormSourceBecomesItemFK : DomainCommand
    {
        public int FormSourceId { private  get; set; }
        public int ItemSourceId { private get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var formSource = ctx.AsQueryable<FormSource>().SingleOrDefault(f => f.Id == FormSourceId);
                if (formSource == null)
                    throw new DomainException($"FormSource with Id '{FormSourceId}' could not be found");

                var itemSource = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.Id == ItemSourceId);
                if (itemSource == null)
                    throw new DomainException($"ItemSource with Id '{ItemSourceId}' could not be found");

                formSource.SetItemSource(itemSource);
                ctx.Update(formSource);
                ctx.Commit();

            };

            ExecuteInternal(context);

        }
    }
}
