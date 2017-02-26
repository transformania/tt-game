using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Skills;
using TT.Domain.Skills.DTOs;

namespace TT.Domain.Skills.Queries
{
    public class GetSkillsPurchaseableByPlayer : DomainQuery<LearnableSkillsDetail>
    {
        public int playerId { get; set; }
        public string MobilityType { get; set; }

        public override IEnumerable<LearnableSkillsDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var ownedSkillSourceIds = ctx.AsQueryable<Skill>()
                           .Include(s => s.SkillSource)
                           .Where(s => s.Owner.Id == playerId && s.SkillSource.MobilityType == MobilityType)
                           .Select(s => s.SkillSource.Id);

                var learnableSkills = ctx.AsQueryable<SkillSource>()
                           .Include(s => s.FormSource)
                           .Where(s => s.IsPlayerLearnable
                                && s.MobilityType == MobilityType
                                && s.FormSource != null
                                && s.IsLive == "live"
                                && (s.LearnedAtLocation != null && s.LearnedAtLocation != "" || s.LearnedAtRegion != null && s.LearnedAtRegion != ""))
                           .ProjectToQueryable<LearnableSkillsDetail>();

                return learnableSkills.Where(x => !ownedSkillSourceIds.Contains(x.Id));

            };

            return ExecuteInternal(context);
        }
    }
}
