using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.ViewModels
{
    public class FriendPageViewModel
    {
        public IEnumerable<FriendPlayerViewModel> ConfirmedFriends;
        public IEnumerable<FriendPlayerViewModel> RequestsForMe;
        public IEnumerable<FriendPlayerViewModel> MyOutgoingRequests;
    }
}