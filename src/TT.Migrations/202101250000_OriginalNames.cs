using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202101250000)]
    public class OriginalNames : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Players").AddColumn("OriginalFirstName").AsCustom("nvarchar(MAX)").Nullable();
            Alter.Table("Players").AddColumn("OriginalLastName").AsCustom("nvarchar(MAX)").Nullable();

            Execute.Sql("UPDATE Players SET OriginalFirstName = FirstName, OriginalLastName = LastName");
        }
    }
}
