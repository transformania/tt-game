using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace tfgame.dbModels.Models
{
    public class Friend
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMembershipId { get; set; }
        [StringLength(128)]
        public string FriendMembershipId { get; set; }
        public DateTime FriendsSince { get; set; }
        public bool IsAccepted { get; set; }
        public string OwnerNicknameForFriend { get; set; }
        public string FriendNicknameForOwner { get; set; }
    }
}