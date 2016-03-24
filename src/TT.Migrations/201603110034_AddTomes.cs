using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201603110034)]
    public class AddTomes : Migration
    {
        public override void Up()
        {
            Create.Table("Tomes")
                .WithColumn("Id").AsInt32().Identity().NotNullable().PrimaryKey()
                .WithColumn("Text").AsString().NotNullable()
                .WithColumn("BaseItem_Id").AsInt32().NotNullable().ForeignKey("DbStaticItems", "Id").Unique();
        }

        public override void Down()
        {

        }
    }
}