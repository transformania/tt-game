using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Commands.Assets
{
    public class DeleteRestockItem : DomainCommand
    {
        public int RestockItemId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var deleteMe = ctx.AsQueryable<RestockItem>().FirstOrDefault(cr => cr.Id == RestockItemId);

                if (deleteMe == null)
                    throw new DomainException(string.Format("RestockItem with ID {0} was not found", RestockItemId));

                ctx.Remove(deleteMe);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RestockItemId <= 0)
                throw new DomainException("RestockItem Id must be greater than 0");
        }
    }
}
