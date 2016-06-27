using TT.Domain.Entities.Effects;

namespace TT.Tests.Builders.Effects
{
    public class EffectBuilder : Builder<Effect, int>
    {
        public EffectBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }
    }
}
