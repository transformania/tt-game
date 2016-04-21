using Highway.Data;
using System.Linq;
using TT.Domain.Entities.NPCs;

namespace TT.Domain.Commands.AI
{
    public class UpdateNPC : DomainCommand
    {
        public int NPCId { get; set; }
        public string SpawnText { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var npc = ctx.AsQueryable<NPC>().SingleOrDefault(n => n.Id == NPCId);

                npc.Update(this);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }
}
