using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Skills;
using TT.Domain.Skills.DTOs;

namespace TT.Domain.Skills.Queries
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
                            .Include(s => s.SkillSource.FormSource)
                            .Include(s  => s.SkillSource.FormSource.ItemSource)
                            .Include(s => s.SkillSource.FormSource.Gender)
                            .Include(s => s.SkillSource.GivesEffectSource)
                           .Where(s => s.Owner.Id == playerId)
                           .ProjectToQueryable<SkillSourceFormSourceDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
