using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Commands
{
    public class GiveRune : DomainCommand
    {

        public int PlayerId { get; set; }
        public int ItemSourceId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>().SingleOrDefault(cr => cr.Id == PlayerId);
                if (player == null)
                    throw new DomainException($"Player with ID '{PlayerId}' could not be found");

               var runeSource = ctx.AsQueryable<ItemSource>().SingleOrDefault(cr => cr.Id == ItemSourceId);
               if (runeSource == null)
                   throw new DomainException($"ItemSource with ID '{ItemSourceId}' could not be found");

                var item = Item.Create(player, runeSource);

                player.GiveItem(item);

                ctx.Update(player);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (PlayerId <= 0)
                throw new DomainException("PlayerId is required!");

            if (ItemSourceId <= 0)
                throw new DomainException("ItemId is required!");
        }



    }
}
