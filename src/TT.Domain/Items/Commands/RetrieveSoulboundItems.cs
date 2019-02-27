using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Commands
{
    public class RetrieveSoulboundItems : DomainCommand<string>
    {

        public int PlayerId { get; set; }
        
        public override string Execute(IDataContext context)
        {

            var output = String.Empty;

            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>()
                    .Include(p => p.Items)
                    .Include(p => p.Items.Select(i => i.ItemSource))
                .SingleOrDefault(p => p.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"player with ID '{PlayerId}' could not be found");

                var soulbindingNPC = ctx.AsQueryable<Player>()
                    .Include(p => p.Items)
                    .Include(p => p.Items.Select(i => i.SoulboundToPlayer))
                    .Include(p => p.Items.Select(i => i.ItemSource))
                    .Include(p => p.Items.Select(i => i.Runes))
                    .SingleOrDefault(p => p.BotId == AIStatics.SoulbinderBotId);

                if (soulbindingNPC == null)
                    throw new DomainException("Soulbinder has not yet spawned.");

                if (player.Location != soulbindingNPC.Location)
                    throw new DomainException($"You must be in the same location as {soulbindingNPC.GetFullName()} to retrieve your soulbound items.");

                if (player.Mobility != PvPStatics.MobilityFull)
                    throw new DomainException("You must be animate in order to do this.");

                output = $"{soulbindingNPC.GetFullName()} returns your soulbound ";


                var itemsToTransfer = soulbindingNPC.Items
                    .Where(i => i.SoulboundToPlayer != null && i.SoulboundToPlayer.Id == PlayerId).ToList();

                if (itemsToTransfer.Count > 0)
                {
                    var itemNames = new List<string>();

                    foreach (var item in itemsToTransfer)
                    {

                        if (item.ItemSource.ItemType == PvPStatics.ItemType_Pet &&
                            player.Items.Count(i => i.ItemSource.ItemType == PvPStatics.ItemType_Pet) > 0)
                        {
                            continue;
                        }

                        item.ChangeOwner(player);
                        itemNames.Add(item.ItemSource.FriendlyName);
                    }

                    output += $"{ListifyHelper.Listify(itemNames)}.";
                }
                else
                {
                    output = $"{soulbindingNPC.GetFullName()} isn't holding any of your soulbound items.";
                }
               

                ctx.Commit();
            };

            ExecuteInternal(context);

            return output;
        }

    }

}