using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class BlacklistProcedures
    {

        public static IEnumerable<BlacklistEntryViewModel> GetMyBlacklistEntries(Player player)
        {
            IBlacklistEntryRepository repo = new EFBlacklistEntryRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            var rawEntries = repo.BlacklistEntries.Where(e => e.CreatorMembershipId == player.MembershipId).ToList();
            var output = new List<BlacklistEntryViewModel>();

            foreach (var e in rawEntries) {
                var addme = new BlacklistEntryViewModel
                {
                    dbBlacklistEntry = e,
                };

                var target = playerRepo.Players.FirstOrDefault(p => p.MembershipId == e.TargetMembershipId);

                if (target != null)
                {
                    addme.PlayerName = target.GetFullName();
                    addme.PlayerId = target.Id;
                }
                else
                {
                    addme.PlayerName = "(You do not currently have any players on your blacklist.)";
                    addme.PlayerId = -1;
                }

                output.Add(addme);
            }

            return output;

        }

        public static string IsAttackBlacklisted(Player player, Player target)
        {
            IBlacklistEntryRepository repo = new EFBlacklistEntryRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            var getEntry = repo.BlacklistEntries.FirstOrDefault(e => e.CreatorMembershipId == player.MembershipId && e.TargetMembershipId == target.MembershipId);

            string output;

            if (getEntry != null)
            {
                output = getEntry.BlacklistLevel.ToString();
            }
            else
            {
                output = "false";
            }

            return output;

        }

        public static string TogglePlayerBlacklist(Player creator, Player receiver, int type, string note)
        {
            IBlacklistEntryRepository repo = new EFBlacklistEntryRepository();
            var entry = repo.BlacklistEntries.FirstOrDefault(e => e.CreatorMembershipId == creator.MembershipId && e.TargetMembershipId == receiver.MembershipId);
            var checkReceiver = repo.BlacklistEntries.FirstOrDefault(e => e.CreatorMembershipId == receiver.MembershipId && e.TargetMembershipId == creator.MembershipId);

            var result = "";

            if (entry != null)
            {
                repo.DeleteBlacklistEntry(entry.Id);
                return receiver.GetFullName() + " has been REMOVED from your blacklist.";
            }
            else
            {
                var newentry = new BlacklistEntry
                {
                    CreatorMembershipId = creator.MembershipId,
                    TargetMembershipId = receiver.MembershipId,
                    Timestamp = DateTime.UtcNow,
                    BlacklistLevel = type,
                    Note = note
                };
                repo.SaveBlacklistEntry(newentry);

                result = "You have blacklisted " + receiver.GetFullName() + ". ";

                if (type == 0)
                {
                    result = result + "They will be unable to attack you. ";
                }
                else if (type == 1)
                {
                    result = result + "They will not be able to message nor attack you. ";
                }
            }

            // If blocking messages or attacks, do it for both people.
            if (checkReceiver == null)
            {
                var mirrorentry = new BlacklistEntry
                {
                    CreatorMembershipId = receiver.MembershipId,
                    TargetMembershipId = creator.MembershipId,
                    Timestamp = DateTime.UtcNow,
                    BlacklistLevel = type,
                    Note = note
                };
                repo.SaveBlacklistEntry(mirrorentry);

                result = result + "You are now blacklisted by them, as well.";
            }

            return result;
        }

        public static string TogglePlayerBlacklistType(int id, string type, Player player, Player receiver)
        {
            IBlacklistEntryRepository repo = new EFBlacklistEntryRepository();

            if ( id == 0 && (player.MembershipId != null && receiver.MembershipId != null))
            {
                var getBlacklistId = repo.BlacklistEntries.FirstOrDefault(e => e.CreatorMembershipId == player.MembershipId && e.TargetMembershipId == receiver.MembershipId);
                id = getBlacklistId.Id;
            }

            var entry = repo.BlacklistEntries.FirstOrDefault(e => e.Id == id);

            if (entry == null)
            {
                return "You have not blacklisted this player.  You must blacklist them before you can change the blacklist type.";
            }

            if (entry.CreatorMembershipId != player.MembershipId)
            {
                return "This is not your blacklist entry.";
            }

            var result = "No change.";

            if (type == "noAttackOnly")
            {
                entry.BlacklistLevel = 0;
                repo.SaveBlacklistEntry(entry);
                result = receiver.GetFullName() + " is now allowed to message you but not attack you. ";
            }
            else if (type == "noAttackOrMessage")
            {
                entry.BlacklistLevel = 1;
                repo.SaveBlacklistEntry(entry);
                result = receiver.GetFullName() + " is not allowed to message OR attack you. ";
            }

            // Blacklist should be in place, but if not.
            var mirrorCheck = repo.BlacklistEntries.FirstOrDefault(f => f.CreatorMembershipId == receiver.MembershipId && f.TargetMembershipId == player.MembershipId);
            if (mirrorCheck == null)
            {
                var mirrorentry = new BlacklistEntry
                {
                    CreatorMembershipId = receiver.MembershipId,
                    TargetMembershipId = player.MembershipId,
                    Timestamp = DateTime.UtcNow,
                    BlacklistLevel = entry.BlacklistLevel
                };

                repo.SaveBlacklistEntry(mirrorentry);

                result = result + "You are now blacklisted by them, as well.";
            }

            return result;

        }

        public static bool PlayersHaveBlacklistedEachOther(Player sender, Player receiver, string type)
        {
            IBlacklistEntryRepository repo = new EFBlacklistEntryRepository();
            var entry = repo.BlacklistEntries.FirstOrDefault(e => (e.CreatorMembershipId == receiver.MembershipId && e.TargetMembershipId == sender.MembershipId) || (e.CreatorMembershipId == sender.MembershipId && e.TargetMembershipId == receiver.MembershipId));

            if (entry == null)
            {
                return false;
            }

            if (type == "message")
            {
                if (entry.BlacklistLevel == 1) // 0 == No attacking, // 1 == No attacking or messaging
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            return true;

           
        }

    }
}