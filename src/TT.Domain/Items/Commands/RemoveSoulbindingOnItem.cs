using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Items.Commands
{
    public class RemoveSoulbindingOnItem : DomainCommand
    {

        public int ItemId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var soulboundItem = ctx.AsQueryable<Item>()
                    .Include(i => i.SoulboundToPlayer)
                    .Include(i => i.FormerPlayer)
                    .Include(i => i.Owner)
                    .Include(i => i.Runes)
                    .Include(i => i.ItemSource)
                    .SingleOrDefault(i => i.Id == ItemId);

                if (soulboundItem == null)
                    throw new DomainException($"Item with Id '{ItemId}' could not be found.");


                soulboundItem.SoulbindToPlayer(null);
                soulboundItem.SetSoulbindingConsent(false);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }
}
