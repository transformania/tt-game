using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.AI;
using TT.Domain.Entities.NPCs;

namespace TT.Domain.Queries.AI
{
    public class GetNPC : DomainQuerySingle<NPCDetail>
    {
        public int NPCId { get; set; }

        public override NPCDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<NPC>()
                            .Where(cr => cr.Id == NPCId)
                            .ProjectToFirstOrDefault<NPCDetail>();
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (NPCId <= 0)
                throw new DomainException("NPC Id must be greater than 0");
        }
    }
}
