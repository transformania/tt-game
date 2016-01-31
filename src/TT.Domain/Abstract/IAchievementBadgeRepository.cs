using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IAchievementBadgeRepository
    {

        IQueryable<AchievementBadge> AchievementBadges { get; }

        void SaveAchievementBadge(AchievementBadge AchievementBadge);

        void DeleteAchievementBadge(int AchievementBadgeId);

    }
}