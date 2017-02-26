using System.Linq;
using Highway.Data;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;

namespace TT.Domain.Assets.Commands
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
                    throw new DomainException($"RestockItem with ID {RestockItemId} was not found");

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
