using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201612292101)]
    public class ArtistBioFKToUser : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.ForeignKey()
               .FromTable("AuthorArtistBios")
               .ForeignColumn("OwnerMembershipId")
               .ToTable("AspNetUsers")
               .PrimaryColumn("Id").OnDelete(Rule.None);

            Alter.Table("AuthorArtistBios").AddColumn("IsLive").AsBoolean().Nullable().WithDefaultValue(false);

        }
    }
}
