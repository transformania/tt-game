using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201703261037)]
    public class Covenants : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Players").AlterColumn("Covenant").AsInt32().Nullable();
            Execute.Sql("UPDATE Players SET Covenant = NULL WHERE Players.Covenant <= 0");

            Alter.Table("LocationInfoes").AlterColumn("CovenantId").AsInt32().Nullable();
            Execute.Sql("UPDATE LocationInfoes SET CovenantId = NULL WHERE LocationInfoes.CovenantId <= 0");

            Create.ForeignKey()
                .FromTable("Players")
                .ForeignColumn("Covenant")
                .ToTable("Covenants")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            Create.ForeignKey()
                .FromTable("Covenants")
                .ForeignColumn("FounderMembershipId")
                .ToTable("AspNetUsers")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            Create.ForeignKey()
                .FromTable("Covenants")
                .ForeignColumn("LeaderId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

        }
    }
}
