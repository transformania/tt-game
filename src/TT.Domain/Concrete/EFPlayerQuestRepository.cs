using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFPlayerQuestRepository : IPlayerQuestRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<PlayerQuest> PlayerQuests
        {
            get { return context.PlayerQuests; }
        }

        public void SavePlayerQuest(PlayerQuest PlayerQuest)
        {
            if (PlayerQuest.Id == 0)
            {
                context.PlayerQuests.Add(PlayerQuest);
            }
            else
            {
                PlayerQuest editMe = context.PlayerQuests.Find(PlayerQuest.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = PlayerQuest.Name;
                    // dbEntry.Message = PlayerQuest.Message;
                    // dbEntry.TimeStamp = PlayerQuest.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeletePlayerQuest(int id)
        {

            PlayerQuest dbEntry = context.PlayerQuests.Find(id);
            if (dbEntry != null)
            {
                context.PlayerQuests.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}