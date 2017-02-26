using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Commands
{
    public class DeleteItem : DomainCommand
    {
        public int ItemId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var deleteMe = ctx.AsQueryable<Item>().FirstOrDefault(i => i.Id == ItemId);

                if (deleteMe == null)
                    throw new DomainException($"Item with ID {ItemId} was not found");

                ctx.Remove(deleteMe);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (ItemId <= 0)
                throw new DomainException("ItemId must be greater than 0");
        }

    }
}
