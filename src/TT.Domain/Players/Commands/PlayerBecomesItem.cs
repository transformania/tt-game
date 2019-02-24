using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.ViewModels;

namespace TT.Domain.Players.Commands
{
    public class PlayerBecomesItem : DomainCommand<LogBox>
    {

        public int VictimId { get; set; }
        public int? AttackerId { get; set; }
        public int NewFormId { get; set; }
        public BuffBox AttackerBuffs { get; set; }

        public override LogBox Execute(IDataContext context)
        {

            LogBox output = new LogBox();

            ContextQuery = ctx =>
            {

                if (AttackerBuffs == null)
                    throw new DomainException("AttackerBuffs is required");

                var victim = ctx.AsQueryable<Player>()
                    .Include(i => i.Items)
                    .Include(p => p.FormSource)
                    .Include(p => p.Items)
                    .Include(p => p.Items.Select(i => i.ItemSource))
                .SingleOrDefault(cr => cr.Id == VictimId);

                if (victim == null)
                    throw new DomainException($"Player (victim) with ID '{VictimId}' could not be found");

                Player attacker = null;

                if (AttackerId != null)
                {
                    attacker = ctx.AsQueryable<Player>()
                        .Include(i => i.Items)
                        .Include(p => p.FormSource)
                        .Include(p => p.Items)
                        .Include(p => p.Items.Select(i => i.ItemSource))
                    .SingleOrDefault(cr => cr.Id == AttackerId);

                    if (attacker == null)
                        throw new DomainException($"Player (attacker) with ID '{AttackerId}' could not be found");
                }

                var newForm = ctx.AsQueryable<FormSource>()
                    .Include(f => f.ItemSource)
                    .SingleOrDefault(cr => cr.Id == NewFormId);

                if (newForm == null)
                    throw new DomainException($"Form with ID '{NewFormId}' could not be found");

                if (newForm.ItemSource == null)
                {
                    throw new DomainException("Form is not inanimate or pet");
                }

                output = victim.TurnIntoItem(attacker, newForm, newForm.ItemSource, AttackerBuffs);

                ctx.Commit();
                
            };

            ExecuteInternal(context);
            return output;

        }

    }

}
