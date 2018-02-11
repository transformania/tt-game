using System.Data.Entity;
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
        public string LocationOverride { get; set; }

        // TODO:  update / add unit tests
        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var item = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)   
                    .Include(i => i.Runes)
                    .Include(i => i.Runes.Select(r => r.ItemSource))
                    .SingleOrDefault(cr => cr.Id == ItemId);

                if (item == null)
                    throw new DomainException($"Item with ID {ItemId} could not be found");

                var player = ctx.AsQueryable<Player>().SingleOrDefault(p => p.Id == OwnerId);

                if (player == null)
                    throw new DomainException($"player with ID {ItemId} could not be found");

                if (item.Owner != null && item.Owner.Id != OwnerId)
                {
                    throw new DomainException($"player {OwnerId} does not own item {ItemId}");
                }

                item.Drop(player, LocationOverride);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (OwnerId <= 0)
                throw new DomainException("OwnerId is required!");

            if (ItemId <= 0)
                throw new DomainException("OwnerId is required!");
        }

    }
}
