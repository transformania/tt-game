using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201908190406)]
    public class RemoveBecomesItemSourceId : ForwardOnlyMigration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_DbStaticForms_BecomesItemSourceId_DbStaticItems_Id").OnTable("DbStaticForms");
            Delete.Column("BecomesItemSourceId").FromTable("DbStaticForms");
        }
    }
}
