using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Items;
using TT.Domain.Statics;

namespace TT.Domain.Commands.Items
{
    public class MoveAbandonedPetsToWuffie : DomainCommand
    {

        public int WuffieId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var wuffie = ctx.AsQueryable<Entities.Players.Player>().FirstOrDefault(p => p.Id == WuffieId);

                if (wuffie == null)
                    throw new DomainException(string.Format("Could not find Wuffie with Id {0}", WuffieId));

                var cutoff = DateTime.UtcNow.AddHours(-8);

                var pets = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Where(i => i.Owner == null &&
                    i.TimeDropped < cutoff &&
                    i.ItemSource.ItemType == PvPStatics.ItemType_Pet);

                foreach (var pet in pets)
                {
                    pet.PickUp(wuffie);
                    pet.ChangeGameMode(GameModeStatics.Any);
                }

                ctx.Commit();
            };

            ExecuteInternal(context);
        }
    }
}
