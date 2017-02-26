using TT.Domain.Effects.Entities;

namespace TT.Tests.Builders.Effects
{
    public class EffectSourceBuilder : Builder<EffectSource, int>
    {
        public EffectSourceBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }
    }
}
