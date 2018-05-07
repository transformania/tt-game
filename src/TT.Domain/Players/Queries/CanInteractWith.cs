using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Players.Queries
{
    public class CanInteractWith : DomainQuerySingle<bool>
    {
        public int BotId { get; set; }
        public int PlayerId { get; set; }

        public override bool Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var npc = ctx.AsQueryable<Player>()
                    .FirstOrDefault(m => m.BotId == BotId);

                var player = ctx.AsQueryable<Player>()
                    .FirstOrDefault(m => m.Id == PlayerId);

                if (npc == null)
                    throw new DomainException($"NPC with BotId '{BotId}' does not exist");

                if (player == null)
                    throw new DomainException($"Player with Id '{PlayerId}' does not exist");

                var pronoun = npc.Gender == PvPStatics.GenderFemale ? "her" : "him";

                if (player.Mobility != PvPStatics.MobilityFull)
                    throw new DomainException($"You must be animate in order to interact with {npc.GetFullName()}.");

                if (npc.Mobility != PvPStatics.MobilityFull)
                    throw new DomainException($"{npc.GetFullName()} must be animate in order for you to interact with {pronoun}.");

                if (player.InDuel > 0)
                    throw new DomainException($"You must conclude your duel in order to interact with {npc.GetFullName()}.");

                if (player.InQuest > 0)
                    throw new DomainException($"You must conclude your quest in order to interact with {npc.GetFullName()}.");


                if (player.Location != npc.Location)
                {
                   
                    throw new DomainException($"You must be in the same location as {npc.GetFullName()} in order to interact with {pronoun}.");
                }

                return true;
            };

            return ExecuteInternal(context);
        }
    }
}
