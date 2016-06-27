using System.Data;
using FluentMigrator;

namespace TT.Migrations
{

    [Migration(201606231841)]
    public class AddEffectFKs : ForwardOnlyMigration
    {

        public override void Up()
        {

            Create.ForeignKey()
                .FromTable("Effects")
                .ForeignColumn("OwnerId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            Alter.Table("Effects").AddColumn("EffectSourceId").AsInt32().Nullable().ForeignKey("DbStaticEffects", "Id");
            Execute.Sql("UPDATE Effects SET EffectSourceId = DbStaticEffects.Id FROM DBStaticEffects WHERE DbStaticEffects.dbName = Effects.dbName");
            Alter.Table("Effects").AlterColumn("EffectSourceId").AsInt32().NotNullable();
        }

    }
}
