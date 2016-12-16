using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201615152300)]
    public class FormSourceFKtoItemSource : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("DbStaticForms").AddColumn("ItemSourceId").AsInt32().Nullable().ForeignKey("DbStaticItems", "Id");
            Execute.Sql("UPDATE DbStaticForms SET ItemSourceId = DbStaticItems.Id FROM DbStaticItems WHERE DbStaticForms.BecomesItemDbName = DbStaticItems.dbName");

            Create.Index("ix_ItemSourceId").OnTable("DbStaticForms").OnColumn("ItemSourceId").Ascending().WithOptions().NonClustered();

        }
    }
}
