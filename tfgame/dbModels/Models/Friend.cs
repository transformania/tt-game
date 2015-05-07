using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public int OwnerMembershipId { get; set; }
        public int FriendMembershipId { get; set; }
        public DateTime FriendsSince { get; set; }
        public bool IsAccepted { get; set; }
        public string OwnerNicknameForFriend { get; set; }
        public string FriendNicknameForOwner { get; set; }
    }
}