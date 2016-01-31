using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFAchievementBadgeRepository : IAchievementBadgeRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<AchievementBadge> AchievementBadges
        {
            get { return context.AchievementBadges; }
        }

        public void SaveAchievementBadge(AchievementBadge AchievementBadge)
        {
            if (AchievementBadge.Id == 0)
            {
                context.AchievementBadges.Add(AchievementBadge);
            }
            else
            {
                AchievementBadge editMe = context.AchievementBadges.Find(AchievementBadge.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = AchievementBadge.Name;
                    // dbEntry.Message = AchievementBadge.Message;
                    // dbEntry.TimeStamp = AchievementBadge.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteAchievementBadge(int id)
        {

            AchievementBadge dbEntry = context.AchievementBadges.Find(id);
            if (dbEntry != null)
            {
                context.AchievementBadges.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}