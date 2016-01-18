using System.Linq;
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