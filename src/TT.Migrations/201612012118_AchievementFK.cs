using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201612012118)]
    public class AddAchievementFK : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.ForeignKey()
                 .FromTable("Achievements")
                 .ForeignColumn("OwnerMembershipId")
                 .ToTable("AspNetUsers")
                 .PrimaryColumn("Id").OnDelete(Rule.None);
        }
    }
}
