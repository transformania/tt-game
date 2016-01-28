using System.Collections.Generic;

namespace tfgame.ViewModels
{
    public class FriendPageViewModel
    {
        public IEnumerable<FriendPlayerViewModel> ConfirmedFriends;
        public IEnumerable<FriendPlayerViewModel> RequestsForMe;
        public IEnumerable<FriendPlayerViewModel> MyOutgoingRequests;
    }
}