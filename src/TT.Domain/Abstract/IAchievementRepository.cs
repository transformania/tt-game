using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IAchievementRepository
    {

        IQueryable<Achievement> Achievements { get; }

        void SaveAchievement(Achievement Achievement);

        void DeleteAchievement(int AchievementId);

    }
}