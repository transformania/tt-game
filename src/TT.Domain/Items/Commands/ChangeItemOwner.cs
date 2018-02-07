using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Items.Commands
{

    // TODO:  There should be a full "TakeItem" command that runs full validation on the player attempting this action, but for now since bots are using this, keep the scope limited
    public class ChangeItemOwner : DomainCommand
    {
        public int OwnerId { get; set; }
        public int ItemId { get; set; }
        public int? GameMode { get; set; }

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

                item.ChangeOwner(player, GameMode);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }
    }
}
