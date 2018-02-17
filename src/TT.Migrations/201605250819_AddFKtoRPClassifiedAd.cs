using FluentMigrator;
using System.Data;

namespace TT.Migrations
{
    [Migration(201605250819)]
    public class AddFKtoRPClassifiedAd : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.ForeignKey()
                .FromTable("RPClassifiedAds")
                .ForeignColumn("OwnerMembershipId")
                .ToTable("AspNetUsers")
                .PrimaryColumn("Id")
                .OnDelete(Rule.None);
        }
    }
}
