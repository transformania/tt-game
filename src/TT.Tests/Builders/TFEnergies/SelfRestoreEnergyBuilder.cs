using TT.Domain.Entities.TFEnergies;

namespace TT.Tests.Builders.TFEnergies
{
    public class SelfRestoreEnergyBuilder : Builder<SelfRestoreEnergy, int>
    {
        public SelfRestoreEnergyBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }

    }
}
