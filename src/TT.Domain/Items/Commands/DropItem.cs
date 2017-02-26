using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Items.Commands
{
    public class DropItem : DomainCommand
    {
        public int OwnerId { get; set; }
        public int ItemId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var item = ctx.AsQueryable<Item>().SingleOrDefault(cr => cr.Id == ItemId);
                if (item == null)
                    throw new DomainException($"Item with ID {ItemId} could not be found");

                var player = ctx.AsQueryable<Player>().SingleOrDefault(p => p.Id == OwnerId);
                if (player == null)
                    throw new DomainException($"player with ID {ItemId} could not be found");

                if (item.Owner != null && item.Owner.Id != OwnerId)
                {
                    throw new DomainException($"player {OwnerId} does not own item {ItemId}");
                }

                item.Drop(player);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {

        }

    }
}
