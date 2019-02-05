using TT.Domain.Effects.Commands;
using TT.Domain.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Effects.Entities
{
    public class Effect : Entity<int>
    {

        public Player Owner { get; protected set; }
        public int Duration { get; protected set; }
        public bool IsPermanent { get; protected set; }
        public int Level { get; protected set; }
        public int Cooldown { get; protected set; }
        public EffectSource EffectSource { get; protected set; }

        private Effect() { }

        public static Effect Create(Player player, EffectSource effectSource, CreateEffect cmd)
        {
            var effect = new Effect
            {
                Owner = player,
                EffectSource = effectSource,
                Duration = cmd.Duration,
                Level = cmd.Level,
                Cooldown = cmd.Cooldown,
                IsPermanent = cmd.IsPermanent
            };
            return effect;
        }
    }
}
