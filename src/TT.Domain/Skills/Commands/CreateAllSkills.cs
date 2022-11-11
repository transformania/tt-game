using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Skills;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;

namespace TT.Domain.Skills.Commands
{
    public class CreateAllSkills : DomainCommand<int>
    {
        public int ownerId { get; set; }
        public IEnumerable<Domain.Skills.DTOs.LearnableSkillsDetail> skillList { get; set; }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>().SingleOrDefault(t => t.Id == ownerId);

                if (player == null)
                {
                    throw new DomainException($"Player with Id {ownerId} could not be found");
                }

                foreach (var currentSkill in skillList)
                {
                    var skillSourceId = currentSkill.Id;
                    var skillSource = ctx.AsQueryable<SkillSource>().SingleOrDefault(t => t.Id == skillSourceId);
                    if (skillSource == null) {
                        throw new DomainException($"StaticSkill Source with Id {skillSourceId} could not be found");
                    }
                    var skill = Skill.CreateAll(player, skillSource, this);
                    ctx.Add(skill);
                }

                ctx.Commit();
                result = ownerId;
            };

            ExecuteInternal(context);
            return result;
        }
    }
}
