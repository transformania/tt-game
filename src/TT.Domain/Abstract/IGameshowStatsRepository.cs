using System.Linq;
using tfgame.dbModels.Models;


namespace tfgame.dbModels.Abstract
{
    public interface IGameshowStatsRepository
    {
        IQueryable<GameshowStats> GameshowStatss { get; }

        void SaveGameshowStats(GameshowStats GameshowStats);

        void DeleteGameshowStats(int GameShowStatsId);
    }
}