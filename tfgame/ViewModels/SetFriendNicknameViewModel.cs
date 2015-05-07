using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class SetFriendNicknameViewModel
    {
        public Player Owner { get; set; }
        public int OwnerMembershipId { get; set; }
        public Player Friend { get; set; }
        public int FriendMembershipId { get; set; }
        public string Nickname { get; set; }
        public int FriendshipId { get; set; }
    }
}