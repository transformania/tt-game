using System.Data.Entity;
using System.Linq;
using Highway.Data;

namespace TT.Domain.Commands.Players
{
    public class DropAllItems : DomainCommand
    {

        public int PlayerId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {


                var player = ctx.AsQueryable<Entities.Players.Player>()
                    .Include(i => i.Items)
                    .SingleOrDefault(p => p.Id == PlayerId);

                if (player == null)
                    throw new DomainException(string.Format("player with ID {0} could not be found", PlayerId));

                player.DropAllItems();
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }
}
