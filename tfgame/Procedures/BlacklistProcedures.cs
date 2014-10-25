using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures
{
    public static class BlacklistProcedures
    {
        public static void AddPlayerToBlacklist(Player creator, Player target)
        {

            IBlacklistEntryRepository repo = new EFBlacklistEntryRepository();

            BlacklistEntry possiblyentry = repo.BlacklistEntries.FirstOrDefault(e => e.CreatorMembershipId == creator.MembershipId && e.TargetMembershipId == target.MembershipId);

            if (possiblyentry == null)
            {
                BlacklistEntry entry = new BlacklistEntry
                {
                    CreatorMembershipId = creator.MembershipId,
                    TargetMembershipId = target.MembershipId,
                    Timestamp = DateTime.UtcNow,
                };
                repo.SaveBlacklistEntry(entry);
            }
        }

        public static void RemovePlayerFromBlacklist(Player creator, Player target)
        {
            IBlacklistEntryRepository repo = new EFBlacklistEntryRepository();
            BlacklistEntry entry = repo.BlacklistEntries.FirstOrDefault(e => e.CreatorMembershipId == creator.MembershipId && e.TargetMembershipId == target.MembershipId);
            repo.DeleteBlacklistEntry(entry.Id);
        }

        public static string TogglePlayerBlacklist(Player creator, Player receiver)
        {
            IBlacklistEntryRepository repo = new EFBlacklistEntryRepository();
            BlacklistEntry entry = repo.BlacklistEntries.FirstOrDefault(e => e.CreatorMembershipId == creator.MembershipId && e.TargetMembershipId == receiver.MembershipId);

            if (entry != null)
            {
                repo.DeleteBlacklistEntry(entry.Id);
                return receiver.GetFullName() + " has been REMOVED from your blacklist.";
            }
            else
            {
                BlacklistEntry newentry = new BlacklistEntry
                {
                    CreatorMembershipId = creator.MembershipId,
                    TargetMembershipId = receiver.MembershipId,
                    Timestamp = DateTime.UtcNow,
                };
                repo.SaveBlacklistEntry(newentry);
                return receiver.GetFullName() + " has been ADDED to your blacklist.";
            }

        }

        public static bool PlayersHaveBlacklistedEachOther(Player sender, Player receiver)
        {
            IBlacklistEntryRepository repo = new EFBlacklistEntryRepository();
            BlacklistEntry entry = repo.BlacklistEntries.FirstOrDefault(e => (e.CreatorMembershipId == receiver.MembershipId && e.TargetMembershipId == sender.MembershipId) || (e.CreatorMembershipId == sender.MembershipId && e.TargetMembershipId == receiver.MembershipId));
            if (entry != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}