using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201805201803)]
    public class AddMotorcycleBoss : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("PvPWorldStats").AddColumn("Boss_MotorcycleGang").AsString().Nullable();
        }
    }
}
