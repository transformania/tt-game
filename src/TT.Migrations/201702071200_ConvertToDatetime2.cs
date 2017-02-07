using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201702071200)]
    public class ConvertToDatetime2 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Achievements")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("AIDirectives")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("ASPNetUsers")
                .AlterColumn("CreateDate").AsCustom("datetime2").Nullable()
                .AlterColumn("LastPasswordFailureDate").AsCustom("datetime2").Nullable()
                .AlterColumn("PasswordChangedDate").AsCustom("datetime2").Nullable()
                .AlterColumn("PasswordVerificationTokenExpirationDate").AsCustom("datetime2").Nullable();

            Alter.Table("AuthorArtistBios")
                .AlterColumn("LastUpdated").AsCustom("datetime2").NotNullable();

            Alter.Table("BlacklistEntries")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("BookReadings")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("BossDamages")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("ChatLogs")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("Contributions")
                .AlterColumn("CreationTimestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("CaptchaEntries")
                .AlterColumn("ExpirationTimestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("Characters")
                .AlterColumn("LastDbSave").AsCustom("datetime2").NotNullable();

            Alter.Table("ChatRooms")
                .AlterColumn("CreatedAt").AsCustom("datetime2").NotNullable();

            Alter.Table("CovenantApplications")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("CovenantLogs")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("Covenants")
                .AlterColumn("LastMemberAcceptance").AsCustom("datetime2").NotNullable();

            Alter.Table("Duels")
                .AlterColumn("LastResetTimestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("EffectContributions")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("Friends")
                .AlterColumn("FriendsSince").AsCustom("datetime2").NotNullable();

            Alter.Table("Furnitures")
                .AlterColumn("LastUseTimestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("InanimateXPs")
                .AlterColumn("LastActionTimestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("Items")
                .AlterColumn("LastSold").AsCustom("datetime2").NotNullable()
                .AlterColumn("LastSouledTimestamp").AsCustom("datetime2").NotNullable()
                .AlterColumn("TimeDropped").AsCustom("datetime2").NotNullable();

            Alter.Table("ItemTransferLogs")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("LocationLogs")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("Messages")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Delete.Index("ix_Timestamp").OnTable("NewsPosts");
            Alter.Table("NewsPosts")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();
            Create.Index("ix_Timestamp").OnTable("NewsPosts")
                .OnColumn("Timestamp").Descending().WithOptions().NonClustered();

            Alter.Table("PlayerBios")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Delete.Index("ix_Timestamp").OnTable("PlayerLogs");
            Alter.Table("PlayerLogs")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();
            Create.Index("ix_Timestamp").OnTable("PlayerLogs")
                .OnColumn("Timestamp").Descending().WithOptions().NonClustered();

            Delete.Index("ix_OnlineActivityTimestamp").OnTable("Players");
            Delete.Index("ix_LastActionTimestamp").OnTable("Players");
            Alter.Table("Players")
                .AlterColumn("LastActionTimestamp").AsCustom("datetime2").NotNullable()
                .AlterColumn("LastCombatAttackedTimestamp").AsCustom("datetime2").NotNullable()
                .AlterColumn("LastCombatTimestamp").AsCustom("datetime2").NotNullable()
                .AlterColumn("OnlineActivityTimestamp").AsCustom("datetime2").NotNullable();
            Create.Index("ix_OnlineActivityTimestamp").OnTable("Players")
                .OnColumn("OnlineActivityTimestamp").Descending().WithOptions().NonClustered();
            Create.Index("ix_LastActionTimestamp").OnTable("Players")
                .OnColumn("LastActionTimestamp").Descending().WithOptions().NonClustered();

            Alter.Table("PollEntries")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("PvPWorldStats")
                .AlterColumn("LastUpdateTimestamp").AsCustom("datetime2").NotNullable()
                .AlterColumn("LastUpdateTimestamp_Finished").AsCustom("datetime2").NotNullable();

            Alter.Table("QuestWriterLogs")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("Rerolls")
                .AlterColumn("LastCharacterCreation").AsCustom("datetime2").NotNullable();

            Alter.Table("ReservedNames")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("RPClassifiedAds")
                .AlterColumn("CreationTimestamp").AsCustom("datetime2").NotNullable()
                .AlterColumn("RefreshTimestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("ServerLogs")
                .AlterColumn("FinishTimestamp").AsCustom("datetime2").NotNullable()
                .AlterColumn("StartTimestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("TFEnergies")
                .AlterColumn("Timestamp").AsCustom("datetime2").NotNullable();

            Alter.Table("VersionInfo")
                .AlterColumn("AppliedOn").AsCustom("datetime2").Nullable();
        }
    }
}