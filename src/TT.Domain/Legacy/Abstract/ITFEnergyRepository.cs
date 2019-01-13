using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface ITFEnergyRepository
    {

        IQueryable<Models.TFEnergy> TFEnergies { get; }

        void SaveTFEnergy(Models.TFEnergy TFEnergy);

        void DeleteTFEnergy(int TFEnergyId);

    }
}