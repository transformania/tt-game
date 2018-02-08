using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Commands
{
    public class UnbembedRunesOnItem : DomainCommand
    {
        public int ItemId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var item = ctx.AsQueryable<Item>()
                    .Include(i => i.Owner)
                    .Include(i => i.ItemSource)
                    .Include(i => i.Runes)
                    .Include(i => i.Runes.Select(r => r.ItemSource))
                    .SingleOrDefault(cr => cr.Id == ItemId);

                if (item == null)
                    throw new DomainException($"Cannot find an item with id '{ItemId}'");

                item.RemoveRunes();

                ctx.Update(item);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (ItemId <= 0)
                throw new DomainException("ItemId is required");
        }
    }
}
