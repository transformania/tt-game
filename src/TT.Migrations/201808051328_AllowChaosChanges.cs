using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201808051328)]
    public class AllowChaosChanges : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("AspNetUsers").AddColumn("AllowChaosChanges").AsBoolean().WithDefaultValue(false);
        }
    }
}
