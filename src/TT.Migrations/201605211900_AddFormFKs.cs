using FluentMigrator;
using System.Data;

namespace TT.Migrations
{
    [Migration(201605211900)]
    public class AddFormFKs : ForwardOnlyMigration
    {
        
        public override void Up()
        {
            Alter.Table("Players").AlterColumn("FormSourceId").AsInt32().ForeignKey("DbStaticForms", "Id");
            Execute.Sql("UPDATE Players SET FormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticForms.dbName = Players.Form");
        }

    }
}
