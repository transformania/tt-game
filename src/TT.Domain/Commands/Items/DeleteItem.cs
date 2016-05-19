using System.Linq;
using Highway.Data;

namespace TT.Domain.Commands.Items
{
    public class DeleteItem : DomainCommand
    {
        public int ItemId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var deleteMe = ctx.AsQueryable<Entities.Items.Item>().FirstOrDefault(i => i.Id == ItemId);

                if (deleteMe == null)
                    throw new DomainException(string.Format("Item with ID {0} was not found", ItemId));

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
