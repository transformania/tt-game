using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201807120100)]
    public class NukeCovenantFounder : ForwardOnlyMigration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_Covenants_FounderMembershipId_AspNetUsers_Id").OnTable("Covenants");
            Delete.Column("FounderMembershipId").FromTable("Covenants");
        }
    }
}
