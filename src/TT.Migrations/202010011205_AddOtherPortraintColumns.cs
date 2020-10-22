using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202010011205)]
    public class AddOtherPortraintColumns : ForwardOnlyMigration
    {
        public override void Up()
        {
        Alter.Table("DbStaticForms").AddColumn("SecondaryPortraitUrl").AsString().Nullable();
        Alter.Table("DbStaticForms").AddColumn("TertiaryPortraitUrl").AsString().Nullable();
        }
    }
}
