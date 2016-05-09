using TT.Domain.Entities.Items;
using System.Linq;
using Highway.Data;

namespace TT.Domain.Commands.Items
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
                    throw new DomainException(string.Format("Item with ID {0} could not be found", ItemId));

                var player = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(p => p.Id == OwnerId);
                if (player == null)
                    throw new DomainException(string.Format("player with ID {0} could not be found", ItemId));

                if (item.Owner.Id != OwnerId)
                {
                    throw new DomainException(string.Format("player {0} does not own item {1}", OwnerId, ItemId));
                }

                var owner = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(p => p.Id == OwnerId);

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
