using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201603181738)]
    public class AddTopicToChatRoom : Migration
    {
        public override void Up()
        {
            Alter.Table("ChatRooms").AddColumn("Topic").AsString().Nullable();

            Update.Table("ChatRooms")
                .Set(new { Topic = "Transformania Time Global Chat" })
                .Where(new { Name = "Global", Creator_Id = "1" });
        }

        public override void Down()
        {
            
        }
    }
}