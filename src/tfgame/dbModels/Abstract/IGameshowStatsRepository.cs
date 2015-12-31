using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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