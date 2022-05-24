using FluentMigrator;
using FluentMigrator.SqlServer;

namespace TT.Migrations
{
    [Migration(202205240400)]
    public class IndexTuning : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Index("IX_AIDirectives_DoNotRecycleMe_Timestamp").OnTable("AIDirectives")
                .OnColumn("DoNotRecycleMe").Ascending()
                .OnColumn("Timestamp").Ascending();

            Create.Index(
                    "IX_Contributions_IsReadyForReview_AdminApproved_ProofreadingCopy_IsLive_CreationTimestamp_inc_Form_FriendlyName_AssignedToArtist")
                .OnTable("Contributions")
                .OnColumn("IsReadyForReview").Ascending()
                .OnColumn("AdminApproved").Ascending()
                .OnColumn("ProofreadingCopy").Ascending()
                .OnColumn("IsLive").Ascending()
                .OnColumn("CreationTimestamp").Ascending()
                .Include("Form_FriendlyName").Include("AssignedToArtist");

            Create.Index("IX_CovenantLogs_Timestamp").OnTable("CovenantLogs")
                .OnColumn("Timestamp").Ascending();

            Create.Index("IX_DbStaticForms_AltSexFormSourceId").OnTable("DbStaticForms")
                .OnColumn("AltSexFormSourceId").Ascending();

            Create.Index("IX_DbStaticSkills_ExclusiveToFormSourceId").OnTable("DbStaticSkills")
                .OnColumn("ExclusiveToFormSourceId").Ascending();
            Create.Index("IX_DbStaticSkills_GivesEffectSourceId").OnTable("DbStaticSkills")
                .OnColumn("GivesEffectSourceId").Ascending();
            Create.Index("IX_DbStaticSkills_IsPlayerLearnable").OnTable("DbStaticSkills")
                .OnColumn("IsPlayerLearnable").Ascending();
            Create.Index("IX_DbStaticSkills_IsPlayerLearnable_inc_IsLive").OnTable("DbStaticSkills")
                .OnColumn("IsPlayerLearnable").Ascending()
                .Include("IsLive");

            Create.Index("IX_Effects_IsPermanent_inc_OwnerId_Duration_Level_Cooldown_EffectSourceId").OnTable("Effects")
                .OnColumn("IsPermanent").Ascending()
                .Include("OwnerId").Include("Duration").Include("Level").Include("Cooldown").Include("EffectSourceId");

            Create.Index("IX_Items_OwnerId_IsEquipped").OnTable("Items")
                .OnColumn("OwnerId").Ascending()
                .OnColumn("IsEquipped").Ascending();
            Create.Index("IX_Items_OwnerId_TimeDropped").OnTable("Items")
                .OnColumn("OwnerId").Ascending()
                .OnColumn("TimeDropped").Ascending();
            Create.Index("IX_Items_OwnerId_TimeDropped_ItemSourceId").OnTable("Items")
                .OnColumn("OwnerId").Ascending()
                .OnColumn("TimeDropped").Ascending()
                .OnColumn("ItemSourceId").Ascending();
            Create.Index(
                    "IX_Items_SoulboundToPlayerId").OnTable("Items")
                .OnColumn("SoulboundToPlayerId").Ascending();
            // This index exceeded the 128 character limit on Indexes, hence KitchenSink
            Create.Index(
                    "IX_Items_SoulboundToPlayerId_inc_OwnerId_dbName_dbLocationName_IsEquipped_TurnsUntilUse_Level_TimeDropped_KitchenSink")
                .OnTable("Items")
                .OnColumn("SoulboundToPlayerId").Ascending()
                .Include("OwnerId").Include("dbName").Include("dbLocationName").Include("IsEquipped")
                .Include("TurnsUntilUse").Include("Level").Include("TimeDropped").Include("EquippedThisTurn")
                .Include("PvPEnabled").Include("IsPermanent").Include("LastSouledTimestamp").Include("LastSold")
                .Include("ItemSourceId").Include("FormerPlayerId").Include("EmbeddedOnItemId")
                .Include("ConsentsToSoulbinding");

            Create.Index("IX_Messages_DoNotRecycleMe_Timestamp").OnTable("Messages")
                .OnColumn("DoNotRecycleMe").Ascending()
                .OnColumn("Timestamp").Ascending();
            Create.Index("IX_Messages_ConversationId_IsDeleted").OnTable("Messages")
                .OnColumn("ConversationId").Ascending()
                .OnColumn("IsDeleted");
            Create.Index("IX_Messages_SenderId_IsDeleted").OnTable("Messages")
                .OnColumn("SenderId").Ascending()
                .OnColumn("IsDeleted");

            Create.Index("IX_PlayerLogs_PlayerId_IsImportant_inc_Message_Timestamp_HideLog").OnTable("PlayerLogs")
                .OnColumn("PlayerId").Ascending()
                .OnColumn("IsImportant").Ascending()
                .Include("Message").Include("Timestamp").Include("HideLog");
            Create.Index("IX_PlayerLogs_PlayerId_inc_Message_Timestamp_IsImportant_HideLog").OnTable("PlayerLogs")
                .OnColumn("PlayerId").Ascending()
                .Include("Message").Include("Timestamp").Include("IsImportant").Include("HideLog");

            Create.Index("IX_Players_Covenant").OnTable("Players")
                .OnColumn("Covenant").Ascending();
            Create.Index("IX_Players_Covenant_BotId").OnTable("Players")
                .OnColumn("Covenant").Ascending()
                .OnColumn("BotId").Ascending();
            Create.Index("IX_Players_Covenant_inc_Mobility").OnTable("Players")
                .OnColumn("Covenant").Ascending()
                .Include("Mobility");
            Create.Index("IX_Players_GameMode").OnTable("Players")
                .OnColumn("GameMode").Ascending();
            Create.Index("IX_Players_GameMode_BotId").OnTable("Players")
                .OnColumn("GameMode").Ascending()
                .OnColumn("BotId").Ascending();
            Create.Index("IX_Players_GameMode_BotId_inc_Mobility_IpAddress").OnTable("Players")
                .OnColumn("GameMode").Ascending()
                .OnColumn("BotId").Ascending()
                .Include("Mobility").Include("IpAddress");
            Create.Index("IX_Players_BotId_inc_Mobility").OnTable("Players")
                .OnColumn("BotId").Ascending()
                .Include("Mobility");
            Create.Index("IX_Players_BotId_OriginalFormSourceId_inc_Mobility").OnTable("Players")
                .OnColumn("BotId").Ascending()
                .OnColumn("OriginalFormSourceId").Ascending()
                .Include("Mobility");
        }
    }
}
