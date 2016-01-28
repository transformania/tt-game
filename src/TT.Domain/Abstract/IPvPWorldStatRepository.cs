using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IPvPWorldStatRepository
    {

        IQueryable<PvPWorldStat> PvPWorldStats { get; }

        void SavePvPWorldStat(PvPWorldStat PvPWorldStat);

        void DeletePvPWorldStat(int PvPWorldStatId);

    }
}