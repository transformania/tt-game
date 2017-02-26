using System;
using System.Linq;
using Highway.Data;
using TT.Domain.Effects.Entities;
using TT.Domain.Entities.Skills;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Items.Entities;

namespace TT.Domain.Skills.Commands
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
                    throw new DomainException($"Skill Source with Id {SkillSourceId} could not be found");

                FormSource formSource = null;
                EffectSource givesEffectSource = null;
                FormSource exclusiveToFormSource = null;
                ItemSource exclusiveToItemSource = null;

                if (!FormSource.IsNullOrEmpty()) { 
                    formSource = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.dbName == FormSource);
                    if (formSource == null)
                    {
                        throw new DomainException(
                            $"FormSource Source with name '{FormSource}' could not be found.  Does it need to be published first?");
                    }
                }

                if (!GivesEffectSource.IsNullOrEmpty())
                {
                    givesEffectSource = ctx.AsQueryable<EffectSource>().SingleOrDefault(t => t.dbName == GivesEffectSource);
                    if (givesEffectSource == null)
                    {
                        throw new DomainException($"EffectSource with name '{GivesEffectSource}' could not be found");
                    }
                }

                if (!ExclusiveToFormSource.IsNullOrEmpty())
                {
                    exclusiveToFormSource = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.dbName == ExclusiveToFormSource);
                    if (exclusiveToFormSource == null)
                    {
                        throw new DomainException(
                            $"ExclusiveToFormSource with name '{ExclusiveToFormSource}' could not be found");
                    }
                }

                if (!ExclusiveToItemSource.IsNullOrEmpty())
                {
                    exclusiveToItemSource = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.DbName == ExclusiveToItemSource);
                    if (exclusiveToFormSource == null)
                    {
                        throw new DomainException(
                            $"ExclusiveToItemSource with name '{ExclusiveToItemSource}' could not be found");
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
