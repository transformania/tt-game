using System.Linq;
using Highway.Data;
using TT.Domain.Entities.NPCs;

namespace TT.Domain.Commands.AI
{
    public class DeleteNPC : DomainCommand
    {
        public int NPCId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var deleteMe = ctx.AsQueryable<NPC>().FirstOrDefault(cr => cr.Id == NPCId);

                if (deleteMe == null)
                    throw new DomainException($"NPC with ID {NPCId} was not found");

                ctx.Remove(deleteMe);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (NPCId <= 0)
                throw new DomainException("NPC Id must be greater than 0");
        }
    }
}
