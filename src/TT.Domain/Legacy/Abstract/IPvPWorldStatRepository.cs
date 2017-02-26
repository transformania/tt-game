using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IPvPWorldStatRepository
    {

        IQueryable<PvPWorldStat> PvPWorldStats { get; }

        void SavePvPWorldStat(PvPWorldStat PvPWorldStat);

        void DeletePvPWorldStat(int PvPWorldStatId);

        void ReloadPvPWorldStat(PvPWorldStat PvPWorldStat);
    }
}