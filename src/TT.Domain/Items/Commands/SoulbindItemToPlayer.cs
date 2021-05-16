using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Services;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Commands
{

    public class SoulbindItemToPlayer : DomainCommand<string>
    {
        public int OwnerId { get; set; }
        public int ItemId { get; set; }

        public override string Execute(IDataContext context)
        {
            var result = "";
            ContextQuery = ctx =>
            {

                var item = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Include(i => i.FormerPlayer)
                    .Include(i => i.Owner)
                    .SingleOrDefault(cr => cr.Id == ItemId);

                if (item == null)
                    throw new DomainException($"Item with ID '{ItemId}' not found.");

                var player = ctx.AsQueryable<Player>()
                    .Include(p => p.Items)
                    .Include(p => p.Items.Select(i => i.SoulboundToPlayer))
                .SingleOrDefault(p => p.Id == OwnerId);

                if (player == null)
                    throw new DomainException($"Player with ID '{OwnerId}' not found.");

                if (player.Level < 4)
                    throw new DomainException("You must be at least level 4 in order to soulbind any items or pets to you.");

                if (item.Owner == null || item.Owner.Id != OwnerId)
                    throw new DomainException("You don't own that item.");

                if (!item.IsPermanent)
                    throw new DomainException("Only permanent items or pets may be souldbound.");

                if (item.FormerPlayer == null)
                    throw new DomainException("Only souled items may be soulbound.");

                if (item.FormerPlayer.BotId == AIStatics.FemaleRatBotId || item.FormerPlayer.BotId == AIStatics.MaleRatBotId)
                {
                    var stat = ctx.AsQueryable<World.Entities.World>()
                        .Include(w => w.Boss_Thief)
                        .FirstOrDefault();

                    if (stat.Boss_Thief == AIStatics.ACTIVE)
                    {
                        throw new DomainException($"You cannot soulbind {item.FormerPlayer.GetFullName()} until both rats have been defeated.");
                    }
                }

                if (!item.ConsentsToSoulbinding)
                    throw new DomainException("This item is not currently consenting to soulbinding.");

                var souledItemCount = ctx.AsQueryable<Item>()
                    .Count(i => i.SoulboundToPlayer != null && i.SoulboundToPlayer.Id == OwnerId);

                var price = PriceCalculator.GetPriceToSoulbindNextItem(souledItemCount);

                if (player.Money < price)
                    throw new DomainException($"You cannot afford this.  You need <b>{price}</b> Arpeyjis and only have <b>{Math.Floor(player.Money)}</b>.");


                item.SoulbindToPlayer(player);
                player.ChangeMoney(-price);

                result = $"You soulbound <b>{item.FormerPlayer.GetFullName()}</b> the <b>{item.ItemSource.FriendlyName}</b> for <b>{price}</b> Arpeyjis.";

                ctx.Commit();
            };

            ExecuteInternal(context);
            return result;
        }

        protected override void Validate()
        {
            if (ItemId <= 0)
                throw new DomainException("ItemId is required");

            if (OwnerId <= 0)
                throw new DomainException("OwnerId is required");

        }
    }
}
