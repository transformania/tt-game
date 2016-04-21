using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201604042151)]
    public class AddNPCs : Migration
    {
        public override void Up()
        {
            Create.Table("NPCs")
                .WithColumn("Id").AsInt32().Identity().NotNullable().PrimaryKey()
                .WithColumn("SpawnText").AsString(int.MaxValue).NotNullable();
        }

        public override void Down()
        {

        }
    }
}
