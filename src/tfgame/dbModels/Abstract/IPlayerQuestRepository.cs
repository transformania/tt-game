using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IPlayerQuestRepository
    {

        IQueryable<PlayerQuest> PlayerQuests { get; }

        void SavePlayerQuest(PlayerQuest PlayerQuest);

        void DeletePlayerQuest(int PlayerQuestId);

    }
}