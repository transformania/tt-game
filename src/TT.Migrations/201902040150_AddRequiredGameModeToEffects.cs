using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201902040150)]
    public class AddRequiredGameModeToEffects : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("DbStaticEffects").AddColumn("RequiredGameMode").AsInt32().Nullable();
            Execute.Sql("UPDATE DbStaticEffects SET RequiredGameMode = '2' WHERE FriendlyName LIKE '% Enchanter'");
        }
    }
}
