using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFAchievementRepository : IAchievementRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Achievement> Achievements
        {
            get { return context.Achievements; }
        }

        public void SaveAchievement(Achievement Achievement)
        {
            if (Achievement.Id == 0)
            {
                context.Achievements.Add(Achievement);
            }
            else
            {
                Achievement editMe = context.Achievements.Find(Achievement.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Achievement.Name;
                    // dbEntry.Message = Achievement.Message;
                    // dbEntry.TimeStamp = Achievement.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteAchievement(int id)
        {

            Achievement dbEntry = context.Achievements.Find(id);
            if (dbEntry != null)
            {
                context.Achievements.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}