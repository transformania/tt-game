using System;
using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.ViewModels;

namespace TT.Domain.Commands.Players
{
    public class ReadjustMaxes : DomainCommand
    {

        public int playerId { get; set; }
        public BuffBox buffs { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(cr => cr.Id == playerId);
                if (player == null)
                    throw new DomainException(string.Format("Player with ID {0} could not be found", playerId));

                player.ReadjustMaxes(buffs);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }
}
