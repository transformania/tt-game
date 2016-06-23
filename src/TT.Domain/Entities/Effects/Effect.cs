using TT.Domain.Commands.Effects;
using TT.Domain.Entities.Players;

namespace TT.Domain.Entities.Effects
{
    public class Effect : Entity<int>
    {

        public Player Owner { get; protected set; }
        public string dbName { get; protected set; }
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
                dbName = effectSource.dbName,
                Duration = cmd.Duration,
                Level = cmd.Level,
                Cooldown = cmd.Cooldown,
                IsPermanent = cmd.IsPermanent
            };
            return effect;
        }
    }
}
