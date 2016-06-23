using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Highway.Data;
using TT.Domain.Entities.Skills;

namespace TT.Domain.Commands.Skills
{
    public class CreateSkill : DomainCommand<int>
    {
        public int ownerId { get; set; }
        public int skillSourceId { get; set; }

        public CreateSkill()
        {
      
           
        }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {
                var skillSource = ctx.AsQueryable<SkillSource>().SingleOrDefault(t => t.Id == skillSourceId);
                if (skillSource == null)
                    throw new DomainException(string.Format("Skill Source with Id {0} could not be found", skillSourceId));

                var player = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(t => t.Id == ownerId);

                if (ownerId != null && player == null)
                {
                    throw new DomainException(string.Format("Player with Id {0} could not be found", ownerId));
                }


                var skill = Skill.Create(player, skillSource, this);

                ctx.Add(skill);
                ctx.Commit();

                result = skill.Id;
            };

            ExecuteInternal(context);

            return result;
        }

    }
}
