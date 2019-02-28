using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201902261940)]
    public class SoulbindItemConsent : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Items").AddColumn("ConsentsToSoulbinding").AsBoolean().WithDefaultValue(false);
        }
    }
}
