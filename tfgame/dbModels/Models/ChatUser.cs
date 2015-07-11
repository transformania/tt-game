using System;

namespace tfgame.dbModels.Models
{
    public class ChatUser
    {
        public int MembershipId { get; private set; }
        public string Name { get; private set; }

        public ChatUser(int membershipId, string name)
        {
            MembershipId = membershipId;
            Name = name;
        }
    }
}