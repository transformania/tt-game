using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IPlayerQuestRepository
    {

        IQueryable<PlayerQuest> PlayerQuests { get; }

        void SavePlayerQuest(PlayerQuest PlayerQuest);

        void DeletePlayerQuest(int PlayerQuestId);

    }
}