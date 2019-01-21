using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Commands
{
    public class MoveAbandonedPetsToWuffie : DomainCommand
    {

        public int WuffieId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var wuffie = ctx.AsQueryable<Player>().FirstOrDefault(p => p.Id == WuffieId);

                if (wuffie == null)
                    throw new DomainException($"Could not find Wuffie with Id {WuffieId}");

                var cutoff = DateTime.UtcNow.AddHours(-8);

                var pets = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Where(i => i.Owner == null &&
                    i.TimeDropped < cutoff &&
                    i.ItemSource.ItemType == PvPStatics.ItemType_Pet);

                foreach (var pet in pets)
                {
                    pet.ChangeOwner(wuffie, (int)GameModeStatics.GameModes.Any);
                }

                ctx.Commit();
            };

            ExecuteInternal(context);
        }
    }
}
