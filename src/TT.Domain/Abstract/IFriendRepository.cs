using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IFriendRepository
    {

        IQueryable<Friend> Friends { get; }

        void SaveFriend(Friend Friend);

        void DeleteFriend(int FriendId);

    }
}