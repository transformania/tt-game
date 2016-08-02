using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201607151721)]
    public class AddTFEnergyFK : ForwardOnlyMigration
    {

        public override void Up()
        {

            Execute.Sql("DELETE [dbo].[TFEnergies] FROM[dbo].[TFEnergies] LEFT OUTER JOIN Players ON TFEnergies.PlayerId = Players.Id WHERE TFEnergies.PlayerId != -1 AND Players.Id IS NULL");

            Execute.Sql("DELETE [dbo].[TFEnergies] FROM[dbo].[TFEnergies] LEFT OUTER JOIN Players ON TFEnergies.CasterId = Players.Id WHERE TFEnergies.CasterId != -1 AND Players.Id IS NULL");

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
