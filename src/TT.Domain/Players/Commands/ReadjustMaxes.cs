using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;
using TT.Domain.ViewModels;

namespace TT.Domain.Players.Commands
{
    /// <summary>
    /// This class is bad and needs to go away.
    /// </summary>
    public class ReadjustMaxes : DomainCommand
    {

        public int playerId { get; set; }
        public BuffBox buffs { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>().SingleOrDefault(cr => cr.Id == playerId);
                if (player == null)
                    throw new DomainException($"Player with ID {playerId} could not be found");

                player.ReadjustMaxes(buffs);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }
}
