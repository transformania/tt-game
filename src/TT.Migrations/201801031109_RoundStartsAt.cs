using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201801031109)]
    public class RoundStartsAt : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("PvPWorldStats").AddColumn("RoundStartsAt").AsCustom("datetime2").Nullable();
        }
    }
}
