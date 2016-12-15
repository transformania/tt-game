using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Skills;
using TT.Domain.Entities.Skills;

namespace TT.Domain.Queries.Skills
{
    public class GetSkillsOwnedByPlayer : DomainQuery<SkillSourceFormSourceDetail>
    {
        public int playerId { get; set; }

        public override IEnumerable<SkillSourceFormSourceDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Skill>()
                            .Include(s => s.SkillSource)
                           .Where(s => s.Owner.Id == playerId)
                           .ProjectToQueryable<SkillSourceFormSourceDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
