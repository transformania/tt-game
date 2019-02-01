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
        public int? FormSourceId { get; set; }
        public int? GivesEffectSourceId { get; set; }
        public int? ExclusiveToFormSourceId { get; set; }
        public string ExclusiveToItemSource { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var skillSource = ctx.AsQueryable<SkillSource>().SingleOrDefault(t => t.Id == SkillSourceId);
                if (skillSource == null)
                    throw new DomainException($"StaticSkill Source with Id {SkillSourceId} could not be found");

                FormSource formSource = null;
                EffectSource givesEffectSource = null;
                FormSource exclusiveToFormSource = null;
                ItemSource exclusiveToItemSource = null;

                if (FormSourceId != null || FormSourceId > 0) { 
                    formSource = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.Id == FormSourceId);
                    if (formSource == null)
                    {
                        throw new DomainException(
                            $"FormSource Source with id '{FormSourceId}' could not be found.  Does it need to be published first?");
                    }
                }

                if (GivesEffectSourceId != null)
                {
                    givesEffectSource = ctx.AsQueryable<EffectSource>().SingleOrDefault(t => t.Id == GivesEffectSourceId);
                    if (givesEffectSource == null)
                    {
                        throw new DomainException($"EffectSource with Id '{GivesEffectSourceId}' could not be found");
                    }
                }

                if (ExclusiveToFormSourceId != null && ExclusiveToFormSourceId.Value > 0)
                {
                    exclusiveToFormSource = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.Id == ExclusiveToFormSourceId);
                    if (exclusiveToFormSource == null)
                    {
                        throw new DomainException(
                            $"ExclusiveToFormSourceId with id '{ExclusiveToFormSourceId}' could not be found");
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
