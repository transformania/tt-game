using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface ITFEnergyRepository
    {

        IQueryable<TFEnergy> TFEnergies { get; }

        void SaveTFEnergy(TFEnergy TFEnergy);

        void DeleteTFEnergy(int TFEnergyId);

    }
}