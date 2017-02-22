using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Commands.Items;
using TT.Domain.Entities.Assets;
using TT.Domain.Entities.Identity;
using TT.Domain.Entities.Items;

namespace TT.Domain.Commands.AI
{
    public class RestockNPC : DomainCommand
    {

        public int BotId { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {

                var npc = ctx.AsQueryable<Entities.Players.Player>()
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
