using System.Linq;
using TT.Domain.Models;


namespace TT.Domain.Abstract
{
    public interface IGameshowStatsRepository
    {
        IQueryable<GameshowStats> GameshowStatss { get; }

        void SaveGameshowStats(GameshowStats GameshowStats);

        void DeleteGameshowStats(int GameShowStatsId);
    }
}