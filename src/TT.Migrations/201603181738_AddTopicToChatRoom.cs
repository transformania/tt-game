using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201603181738)]
    public class AddTopicToChatRoom : Migration
    {
        public override void Up()
        {
            Alter.Table("ChatRooms").AddColumn("Topic").AsString().Nullable();
        }

        public override void Down()
        {
            
        }
    }
}