using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201609271803)]
    public class AddDonatorFK : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.ForeignKey()
                 .FromTable("Donators")
                 .ForeignColumn("OwnerMembershipId")
                 .ToTable("AspNetUsers")
                 .PrimaryColumn("Id").OnDelete(Rule.None);
        }
    }
}
