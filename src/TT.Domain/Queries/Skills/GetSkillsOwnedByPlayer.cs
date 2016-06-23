using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Skills;
using TT.Domain.Entities.Skills;

namespace TT.Domain.Queries.Skills
{
    public class GetSkillsOwnedByPlayer : DomainQuery<SkillDetail>
    {
        public int playerId { get; set; }

        public override IEnumerable<SkillDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Skill>()
                           .Where(s => s.Owner.Id == playerId)
                           .ProjectToQueryable<SkillDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
