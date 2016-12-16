using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Items;

namespace TT.Domain.Commands.Items
{
    public class ChangeItemOwner : DomainCommand
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

                var player = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(p => p.Id == OwnerId);
                if (player == null)
                    throw new DomainException($"player with ID {ItemId} could not be found");

                item.ChangeOwner(player);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }
    }
}
