using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IFriendRepository
    {

        IQueryable<Friend> Friends { get; }

        void SaveFriend(Friend Friend);

        void DeleteFriend(int FriendId);

    }
}