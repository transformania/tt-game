using System.Collections.Generic;

namespace TT.Domain.ViewModels
{
    public class FriendPageViewModel
    {
        public IEnumerable<FriendPlayerViewModel> ConfirmedFriends;
        public IEnumerable<FriendPlayerViewModel> RequestsForMe;
        public IEnumerable<FriendPlayerViewModel> MyOutgoingRequests;
        public bool IsOnlineToggled { get; set; }
        public int PlayerId { get; set; }
        public bool FriendOnlyMessages { get; set; }
    }
}