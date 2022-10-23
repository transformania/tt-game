using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202210212000)]
    public class AddAllowOwnershipVisibilityColumn : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("AspNetUsers").AddColumn("AllowOwnershipVisibility").AsBoolean().WithDefaultValue(false);
        }
    }
}