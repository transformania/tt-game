using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.AI.DTOs;
using TT.Domain.AI.Entities;

namespace TT.Domain.AI.Queries
{
    public class GetNPCs : DomainQuery<NPCDetail>
    {
        public override IEnumerable<NPCDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var npcs = ctx.AsQueryable<NPC>().ToList();
                return npcs.Select(npc => npc.MapToDto()).AsQueryable();
            };
            return ExecuteInternal(context);
        }
    }
}