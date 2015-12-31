using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IAchievementRepository
    {

        IQueryable<Achievement> Achievements { get; }

        void SaveAchievement(Achievement Achievement);

        void DeleteAchievement(int AchievementId);

    }
}