using System;
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

                FormSource formSource = null;
                EffectSource givesEffectSource = null;
                FormSource exclusiveToFormSource = null;
                ItemSource exclusiveToItemSource = null;

                if (!FormSource.IsNullOrEmpty()) { 
                    formSource = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.dbName == FormSource);
                    if (formSource == null)
                    {
                        throw new DomainException(string.Format("FormSource Source with name '{0}' could not be found", FormSource));
                    }
                }

                if (!GivesEffectSource.IsNullOrEmpty())
                {
                    givesEffectSource = ctx.AsQueryable<EffectSource>().SingleOrDefault(t => t.dbName == GivesEffectSource);
                    if (givesEffectSource == null)
                    {
                        throw new DomainException(string.Format("EffectSource with name '{0}' could not be found", GivesEffectSource));
                    }
                }

                if (!ExclusiveToFormSource.IsNullOrEmpty())
                {
                    exclusiveToFormSource = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.dbName == ExclusiveToFormSource);
                    if (exclusiveToFormSource == null)
                    {
                        throw new DomainException(string.Format("ExclusiveToFormSource with name '{0}' could not be found", ExclusiveToFormSource));
                    }
                }

                if (!ExclusiveToItemSource.IsNullOrEmpty())
                {
                    exclusiveToItemSource = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.DbName == ExclusiveToItemSource);
                    if (exclusiveToFormSource == null)
                    {
                        throw new DomainException(string.Format("ExclusiveToItemSource with name '{0}' could not be found", ExclusiveToItemSource));
                    }
                }

                skillSource.SetSources(formSource, givesEffectSource, exclusiveToFormSource, exclusiveToItemSource);

                ctx.Update(skillSource);
                ctx.Commit();

            };

            ExecuteInternal(context);

        }
    }
}
