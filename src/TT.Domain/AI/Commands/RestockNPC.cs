using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;

namespace TT.Domain.AI.Commands
{
    public class RestockNPC : DomainCommand
    {

        public int BotId { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {

                var npc = ctx.AsQueryable<Player>()
                    .Include(n => n.Items)
                    .Include(n => n.Items.Select(i => i.ItemSource))
                    .SingleOrDefault(t => t.BotId == BotId);

                if (npc == null)
                    throw new DomainException($"Player with BotId '{BotId}' could not be found");

                var restockList = ctx.AsQueryable<RestockItem>()
                    .Include(r => r.BaseItem)
                    .Where(r => r.BotId == BotId);

                foreach (var r in restockList)
                {
                    var currentAmount = npc.GetCountOfItem(r.BaseItem.Id);

                    if (currentAmount <= r.AmountBeforeRestock)
                    {
                        var amountRequired = r.AmountToRestockTo - currentAmount;
                        npc.GiveItemsOfType(r.BaseItem, amountRequired);
                    }
                }

                ctx.Commit();

            };

            ExecuteInternal(context);

        }
    }
}
