using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Effects;
using TT.Domain.Entities.Forms;
using TT.Domain.Entities.Item;
using TT.Domain.Entities.Skills;

namespace TT.Domain.Commands.Skills
{
    public class SetSkillSourceFKs : DomainCommand
    {
        public int SkillSourceId { get; set; }
        public string FormSource { get; set; }
        public string GivesEffectSource { get; set; }
        public string ExclusiveToFormSource { get; set; }
        public string ExclusiveToItemSource { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var skillSource = ctx.AsQueryable<SkillSource>().SingleOrDefault(t => t.Id == SkillSourceId);
                if (skillSource == null)
                    throw new DomainException(string.Format("Skill Source with Id {0} could not be found", SkillSourceId));

                var formSource = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.dbName == FormSource);
                var givesEffectSource = ctx.AsQueryable<EffectSource>().SingleOrDefault(t => t.dbName == GivesEffectSource);
                var exclusiveToFormSource = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.dbName == ExclusiveToFormSource);
                var exclusiveToItemSource = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.DbName == ExclusiveToItemSource);

                skillSource.SetSources(formSource, givesEffectSource, exclusiveToFormSource, exclusiveToItemSource);

                ctx.Update(skillSource);
                ctx.Commit();

            };

            ExecuteInternal(context);

        }
    }
}
