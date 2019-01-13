using TT.Domain.Entities.TFEnergies;
using TT.Domain.TFEnergies.Entities;

namespace TT.Tests.Builders.TFEnergies
{
    public class TFEnergyBuilder : Builder<TFEnergy, int>
    {
        public TFEnergyBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }
        
    }
}
