using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class FriendProcedures
    {
        public static void AddFriend(Player player, string membershipId)
        {
            IFriendRepository friendRepo = new EFFriendRepository();

            Friend friend = friendRepo.Friends.FirstOrDefault(f => (f.OwnerMembershipId == membershipId && f.FriendMembershipId == player.MembershipId) || (f.FriendMembershipId == membershipId && f.OwnerMembershipId == player.MembershipId));

            // if friend is null, add a new unnaccepted friend binding
            if (friend == null)
            {
                friend = new Friend();
                friend.OwnerMembershipId = membershipId;
                friend.FriendMembershipId = player.MembershipId;
                friend.IsAccepted = false;
                friend.FriendsSince = DateTime.UtcNow;
                friend.FriendNicknameForOwner = "[UNASSIGNED]";
                friend.OwnerNicknameForFriend = "[UNASSIGNED]";

                friendRepo.SaveFriend(friend);
            }

        }

        public static Friend GetFriend(int friendId)
        {
            IFriendRepository friendRepo = new EFFriendRepository();
            return friendRepo.Friends.FirstOrDefault(f => f.Id == friendId);
        }

        public static string DeleteFriend(int friendId, string membershipId)
        {
            IFriendRepository friendRepo = new EFFriendRepository();


            Friend deleteMe = friendRepo.Friends.FirstOrDefault(f => f.Id == friendId);

            if (deleteMe == null)
            {
                return "Friendship not found.";
            }

            //assert friend is yours
            if (deleteMe.OwnerMembershipId != membershipId)
            {
                return "This is not your friend listing.";
            }

            friendRepo.DeleteFriend(friendId);
            return "Deleted";

        }

        public static bool PlayerIsMyFriend(Player me, Player them)
        {
            return MemberIsMyFriend(me.MembershipId, them.MembershipId);
        }

        public static bool MemberIsMyFriend(string me, string them)
        {
            IFriendRepository friendRepo = new EFFriendRepository();
            Friend dbFriend = friendRepo.Friends.FirstOrDefault(f => f.OwnerMembershipId == me && f.FriendMembershipId == them && f.IsAccepted);
            Friend dbFriend2 = friendRepo.Friends.FirstOrDefault(f => f.OwnerMembershipId == them && f.FriendMembershipId == me && f.IsAccepted);

            if (dbFriend != null || dbFriend2 != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static IEnumerable<FriendPlayerViewModel> GetMyFriends(string membershipId)
        {
            IFriendRepository friendRepo = new EFFriendRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            IEnumerable<Friend> mydbfriends = friendRepo.Friends.Where(f => f.OwnerMembershipId == membershipId || f.FriendMembershipId == membershipId);

            List<FriendPlayerViewModel> output = new List<FriendPlayerViewModel>();

            foreach (Friend friend in mydbfriends)
            {
                FriendPlayerViewModel friendPlayer = new FriendPlayerViewModel();

                // this was a request sent BY me.  Grab the player who it was sent to
                if (friend.OwnerMembershipId == membershipId)
                {

                    Player plyr = playerRepo.Players.FirstOrDefault(p => p.MembershipId == friend.FriendMembershipId);

                    if (plyr != null)
                    {
                        friendPlayer.dbPlayer = plyr;
                        friendPlayer.dbFriend = friend;
                        friendPlayer.friendId = friend.Id;
                        output.Add(friendPlayer);
                    }
                }

                    // this was a request sent TO me.  Grab the player who sent it
                else if (friend.FriendMembershipId == membershipId)
                {
                    Player plyr = playerRepo.Players.FirstOrDefault(p => p.MembershipId == friend.OwnerMembershipId);

                    if (plyr != null)
                    {
                        friendPlayer.dbPlayer = plyr;
                        friendPlayer.dbFriend = friend;
                        friendPlayer.friendId = friend.Id;
                        output.Add(friendPlayer);
                    }
                }
            }

            return output;
        }

        public static string CancelFriendRequest(int id, string membershipId)
        {
            IFriendRepository friendRepo = new EFFriendRepository();
            Friend friend = friendRepo.Friends.FirstOrDefault(f => f.Id == id);


            // assert exists
            if (friend == null)
            {
                return "Error";
            }

            // assert you've sent this, or else it was sent to you
            else if (friend.OwnerMembershipId == membershipId || friend.FriendMembershipId == membershipId)
            {
                friendRepo.DeleteFriend(friend.Id);
                return "";
            }
            else
            {
                return "";
            }


        }

        public static string AcceptFriendRequest(int id, string membershipId)
        {
            IFriendRepository friendRepo = new EFFriendRepository();
            Friend friend = friendRepo.Friends.FirstOrDefault(f => f.Id == id);


            // assert exists
            if (friend == null)
            {
                return "Error";
            }

            // assert this was sent to you
            else if (friend.FriendMembershipId == membershipId)
            {
                friend.IsAccepted = true;
                friendRepo.SaveFriend(friend);
                return "Success";
            }
            else
            {
                return "";
            }
        }

        public static string OwnerSetNicknameOfFriend(int id, string input)
        {
            IFriendRepository friendRepo = new EFFriendRepository();
            Friend friend = friendRepo.Friends.FirstOrDefault(f => f.Id == id);
            friend.OwnerNicknameForFriend = input;
            friendRepo.SaveFriend(friend);
            return "You set the nickname of this friend to '" + input + "'.";
        }

        public static string FriendSetNicknameOfOwner(int id, string input)
        {
            IFriendRepository friendRepo = new EFFriendRepository();
            Friend friend = friendRepo.Friends.FirstOrDefault(f => f.Id == id);
            friend.FriendNicknameForOwner = input;
            friendRepo.SaveFriend(friend);
            return "You set the nickname of this friend to '" + input + "'.";
        }

    }
}