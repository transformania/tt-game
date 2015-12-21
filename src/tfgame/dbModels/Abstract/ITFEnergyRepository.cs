using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface ITFEnergyRepository
    {

        IQueryable<TFEnergy> TFEnergies { get; }

        void SaveTFEnergy(TFEnergy TFEnergy);

        void DeleteTFEnergy(int TFEnergyId);

    }
}