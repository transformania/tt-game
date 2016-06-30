using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201630064900)]
    public class AddTFEnergyFK : ForwardOnlyMigration
    {

        public override void Up()
        {

            Create.ForeignKey()
                .FromTable("TFEnergies")
                .ForeignColumn("PlayerId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            Update.Table("TFEnergies").Set(new { CasterId = null as object }).Where(new { CasterId = -1 });
            Alter.Table("TFEnergies").AlterColumn("CasterId").AsInt32().Nullable().ForeignKey("Players", "Id");

            Alter.Table("TFEnergies").AddColumn("FormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");
            Execute.Sql("UPDATE TFEnergies SET FormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticForms.dbName = TFEnergies.FormName");

        }

    }
}
