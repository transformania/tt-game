using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Combat.DTOs;
using TT.Domain.Entities.MindControl;

namespace TT.Domain.Combat.Queries
{
    public class GetMindControlsForPlayer : DomainQuery<VictimMindControlDetail>
    {

        public int OwnerId { get; set; }

        public override IEnumerable<VictimMindControlDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var vmcs = ctx.AsQueryable<VictimMindControl>()
                    .Include(cr => cr.Victim)
                    .Where(cr => cr.Victim.Id == OwnerId)
                    .ToList();

                return vmcs.Select(cr => cr.MapToDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }

    }
}
