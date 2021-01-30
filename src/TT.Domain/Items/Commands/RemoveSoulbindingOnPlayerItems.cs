
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Commands
{
    public class RemoveSoulbindingOnPlayerItems : DomainCommand
    {
        public int PlayerId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>()
                    .SingleOrDefault(p => p.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"Player with ID '{PlayerId}' could not be found");

                var lindella = ctx.AsQueryable<Player>()
                    .SingleOrDefault(p => p.BotId == AIStatics.LindellaBotId);

                if (lindella == null)
                    throw new DomainException($"Player with Bot Id '{PlayerId}' could not be found");

                var wuffie = ctx.AsQueryable<Player>()
                    .SingleOrDefault(p => p.BotId == AIStatics.WuffieBotId);

                if (wuffie == null)
                    throw new DomainException($"Player with Bot Id '{PlayerId}' could not be found");

                var soulboundItems = ctx.AsQueryable<Item>()
                    .Include(i => i.SoulboundToPlayer)
                    .Include(i => i.FormerPlayer)
                    .Include(i => i.Owner)
                    .Include(i => i.Runes)
                    .Include(i => i.ItemSource)
                    .Where(i => i.SoulboundToPlayer != null && i.SoulboundToPlayer.Id == PlayerId);

                foreach (var item in soulboundItems)
                {
                    item.SoulbindToPlayer(null);

                    if (item.FormerPlayer?.BotId == AIStatics.ActivePlayerBotId)
                    {
                        item.SetSoulbindingConsent(false);
                    }

                    if (item.Owner.BotId == AIStatics.SoulbinderBotId)
                    {
                        if (item.ItemSource.ItemType == PvPStatics.ItemType_Pet)
                        {
                            item.ChangeOwner(wuffie);
                        }
                        else
                        {
                            item.ChangeOwner(lindella);
                        }
                    }
                }

                ctx.Commit();
            };

            ExecuteInternal(context);
        }
    }
}
