using System.Data.Entity;
using System.Data.SqlClient;
using TT.Domain.Concrete;
using TT.Domain.Messages.Commands;
using TT.Domain.Procedures;

namespace TT.Domain.Legacy.Procedures
{
    public static class DeleteAccountProcedures
    {

        public static void DeleteAccount(string membershipId, int playerId)
        {
            DomainRegistry.Repository.Execute(new DeleteAllMessagesOwnedByPlayer { OwnerId = playerId });

            var me = PlayerProcedures.GetPlayerFromMembership(membershipId);
            if (me.Covenant > 0)
            {
                CovenantProcedures.RemovePlayerFromCovenant(me);
            }

            using (var context = new StatsContext())
            {
                context.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction,
                     @"DELETE FROM [dbo].[ContributorCustomForms] WHERE OwnerMembershipId = @membershipId;
                    DELETE FROM [dbo].[Donators] WHERE OwnerMembershipId = @membershipId;
                    DELETE FROM [dbo].[Friends] WHERE OwnerMembershipId = @membershipId OR FriendMembershipId = @membershipId;
                    DELETE FROM [dbo].[PlayerBios] WHERE OwnerMembershipId = @membershipId;
                    DELETE FROM [dbo].[PollEntries] WHERE OwnerMembershipId = @membershipId;
                    DELETE FROM [dbo].[RPClassifiedAds] WHERE OwnerMembershipId = @membershipId;
                    DELETE FROM [dbo].[Strikes] WHERE UserMembershipId = @membershipId OR FromModerator = @membershipId;
                    DELETE FROM [dbo].[Reports] WHERE Reporter = @membershipId OR Reported = @membershipId;
                    DELETE FROM [dbo].[Achievements] WHERE OwnerMembershipId = @membershipId;
                    DELETE FROM [dbo].[AchievementBadges] WHERE OwnerMembershipId = @membershipId;
                    DELETE FROM [dbo].[AspNetUserRoles] WHERE UserId = @membershipId;
                    DELETE FROM [dbo].[AuthorArtistBios] WHERE OwnerMembershipId = @membershipId;
                    DELETE FROM [dbo].[CaptchaEntries] WHERE User_Id = @membershipId;
                    DELETE FROM [dbo].[BlacklistEntries] WHERE CreatorMembershipId = @membershipId OR TargetMembershipId = @membershipId;
                    DELETE FROM [dbo].[ReservedNames] WHERE MembershipId = @membershipId;
                    
                    UPDATE [dbo].[Contributions] SET OwnerMembershipId = NULL WHERE OwnerMembershipId = @membershipId;
                    UPDATE [dbo].[EffectContributions] SET OwnerMemberhipId = NULL WHERE OwnerMemberhipId = @membershipId;
                    UPDATE [dbo].[DMRolls] SET MembershipOwnerId = NULL WHERE MembershipOwnerId = @membershipId;
                    UPDATE [dbo].[Players] SET BotId = -1, IpAddress = CONCAT(IpAddress, '-deleted') WHERE MembershipId = @membershipId;
                    UPDATE [dbo].[Items] SET IsPermanent = 'True' WHERE FormerPlayerId = @playerId;
                    
                    UPDATE [dbo].[Players] SET MembershipId = NULL WHERE MembershipId = @membershipId;
                    
                    DELETE FROM [dbo].[AspNetUsers] WHERE Id = @membershipId;",
                       new SqlParameter("@membershipId", membershipId),
                       new SqlParameter("@playerId", playerId));
            }

        }

    }
}
