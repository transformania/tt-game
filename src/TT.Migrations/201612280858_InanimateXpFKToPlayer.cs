using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201612280858)]
    public class InanimateXpFKToPlayer : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.ForeignKey()
               .FromTable("InanimateXPs")
               .ForeignColumn("OwnerId")
               .ToTable("Players")
               .PrimaryColumn("Id").OnDelete(Rule.None);
        }
    }
}
