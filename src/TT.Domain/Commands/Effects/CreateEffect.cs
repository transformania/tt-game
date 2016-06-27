using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Effects;

namespace TT.Domain.Commands.Effects
{
    public class CreateEffect : DomainCommand<int>
    {

        public int EffectSourceId { get; set; }
        public int OwnerId { get; set; }
        public int Duration { get; set; }
        public bool IsPermanent { get; set; }
        public int Level { get; set; }
        public int Cooldown { get; set; }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {
                var effectSource = ctx.AsQueryable<EffectSource>().SingleOrDefault(t => t.Id == EffectSourceId);
                if (effectSource == null)
                    throw new DomainException(string.Format("Effect Source with Id {0} could not be found", EffectSourceId));

                var player = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(t => t.Id == OwnerId);

                if (player == null)
                {
                    throw new DomainException(string.Format("Player with Id {0} could not be found", OwnerId));
                }

                var item = Effect.Create(player, effectSource, this);

                ctx.Add(item);
                ctx.Commit();

                result = item.Id;
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {

        }
    }
}
