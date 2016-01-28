using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class SetFriendNicknameViewModel
    {
        public Player Owner { get; set; }
        public string OwnerMembershipId { get; set; }
        public Player Friend { get; set; }
        public string FriendMembershipId { get; set; }
        public string Nickname { get; set; }
        public int FriendshipId { get; set; }
    }
}