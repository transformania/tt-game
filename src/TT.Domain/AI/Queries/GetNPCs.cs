using System.Collections.Generic;
using Highway.Data;
using TT.Domain.AI.DTOs;
using TT.Domain.AI.Entities;

namespace TT.Domain.AI.Queries
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