using System.Linq;
using Highway.Data;
using TT.Domain.Effects.Entities;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;

namespace TT.Domain.Effects.Commands
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
                    throw new DomainException($"Effect Source with Id {EffectSourceId} could not be found");

                var player = ctx.AsQueryable<Player>().SingleOrDefault(t => t.Id == OwnerId);

                if (player == null)
                {
                    throw new DomainException($"Player with Id {OwnerId} could not be found");
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
