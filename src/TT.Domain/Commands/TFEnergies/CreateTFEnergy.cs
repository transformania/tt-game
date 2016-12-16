using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Forms;

namespace TT.Domain.Commands.TFEnergies
{
    public class CreateTFEnergy : DomainCommand<int>
    {
        public int PlayerId { get; set; }
        public int? FormSourceId { get; set; }
        public decimal Amount { get; set; }
        public string FormName { get; set; }
        public int? CasterId { get; set; }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(p => p.Id == PlayerId);

                if (player==null)
                    throw new DomainException($"Player with ID {PlayerId} could not be found");

                Entities.Players.Player caster = null;

                FormSource form = null;

                caster = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(p => p.Id == CasterId);

                if (FormSourceId != null)
                {
                    form = ctx.AsQueryable<FormSource>().SingleOrDefault(t => t.Id == FormSourceId);
                }

                var energy = Entities.TFEnergies.TFEnergy.Create(player, caster, form, this);

                ctx.Add(energy);
                ctx.Commit();

                result = energy.Id;

            };

            ExecuteInternal(context);

            return result;

        }

        protected override void Validate()
        {
            if (PlayerId <= 0)
                throw new DomainException("Player Id is required");
        }
    }
}
