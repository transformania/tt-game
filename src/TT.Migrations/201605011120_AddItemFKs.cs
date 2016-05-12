using FluentMigrator;
using System.Data;

namespace TT.Migrations
{
    [Migration(201605011120)]
    public class AddItemFKs : ForwardOnlyMigration
    {
        
        public override void Up()
        {
            Alter.Table("Items").AlterColumn("OwnerId").AsInt32().Nullable();
            Update.Table("Items").Set(new { OwnerId = null as object }).Where(new { OwnerId = -1 });

            Create.ForeignKey()
                .FromTable("Items")
                .ForeignColumn("OwnerId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            Alter.Table("Items").AddColumn("ItemSourceId").AsInt32().ForeignKey("DbStaticItems", "Id");

        }

    }
}
