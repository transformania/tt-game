using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Commands
{
    public class DropAllItems : DomainCommand
    {

        public int PlayerId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {


                var player = ctx.AsQueryable<Player>()
                    .Include(i => i.Items)
                    .SingleOrDefault(p => p.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"player with ID {PlayerId} could not be found");

                player.DropAllItems();
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }
}
