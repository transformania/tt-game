using System.Collections.Generic;
using Highway.Data;
using TT.Domain.DTOs.AI;
using TT.Domain.Entities.NPCs;

namespace TT.Domain.Queries.AI
{
    public class GetNPCs : DomainQuery<NPCDetail>
    {
        public override IEnumerable<NPCDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<NPC>().ProjectToQueryable<NPCDetail>();
            return ExecuteInternal(context);
        }
    }
}