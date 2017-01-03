using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201703083300)]
    public class ChatLogColumnAdditions : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("ChatLogs").AddColumn("UserId").AsString(128).Nullable();
            Alter.Table("ChatLogs").AddColumn("Name").AsString().Nullable();
            Alter.Table("ChatLogs").AddColumn("PortraitUrl").AsString().Nullable();
            Alter.Table("ChatLogs").AddColumn("Color").AsString().Nullable();

            Update.Table("ChatLogs")
                .Set(new { Name = "unknown", Color = "black" })
                .AllRows();
        }
    }
}
