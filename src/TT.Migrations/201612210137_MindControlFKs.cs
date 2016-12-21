using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201612210137)]
    public class MindControlFKs : ForwardOnlyMigration
    {
        public override void Up()
        {

            Create.ForeignKey()
                .FromTable("MindControls")
                .ForeignColumn("MasterId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            Create.ForeignKey()
                .FromTable("MindControls")
                .ForeignColumn("VictimId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

        }
    }
}
