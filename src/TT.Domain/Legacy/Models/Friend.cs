using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
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