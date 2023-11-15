using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202310210935)]
    public class AddFriendMessage : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Players").AddColumn("FriendOnlyMessages").AsBoolean().WithDefaultValue(false);
        }
    }
}