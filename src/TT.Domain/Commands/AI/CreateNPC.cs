
using Highway.Data;
using TT.Domain.Entities.NPCs;

namespace TT.Domain.Commands.AI
{
    public class CreateNPC : DomainCommand<int>
    {
        public string SpawnText { get; set; }

        public override int Execute(IDataContext context)
        {
            int result = 0;

            ContextQuery = ctx =>
            {
               
                var npc = NPC.Create(SpawnText);

                ctx.Add(npc);
                ctx.Commit();

                result = npc.Id;
            };
      
            ExecuteInternal(context);

            return result;
        }

    }
}