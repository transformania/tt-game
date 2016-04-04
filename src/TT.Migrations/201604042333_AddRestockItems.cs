using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201604042333)]
    public class AddRestockItems : Migration
    {
        public override void Up()
        {
            Create.Table("RestockItems")
                .WithColumn("Id").AsInt32().Identity().NotNullable().PrimaryKey()
                .WithColumn("BaseItem_Id").AsInt32().NotNullable().ForeignKey("DbStaticItems", "Id")
                .WithColumn("NPC_Id").AsInt32().NotNullable().ForeignKey("NPCs", "Id")
                .WithColumn("AmountBeforeRestock").AsInt32().NotNullable()
                .WithColumn("AmountToRestockTo").AsInt32().NotNullable();
        }

        public override void Down()
        {

        }
    }
}
