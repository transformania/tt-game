using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IAchievementBadgeRepository
    {

        IQueryable<AchievementBadge> AchievementBadges { get; }

        void SaveAchievementBadge(AchievementBadge AchievementBadge);

        void DeleteAchievementBadge(int AchievementBadgeId);

    }
}