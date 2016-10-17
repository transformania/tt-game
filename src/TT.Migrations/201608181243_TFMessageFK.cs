using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201608181243)]
    public class AddTFMessagesFK : ForwardOnlyMigration
    {
        public override void Up()
        {

            Execute.Sql("DELETE [dbo].[TFMessages] FROM [dbo].[TFMessages] LEFT OUTER JOIN DbStaticForms ON TFMessages.FormDbName = DbStaticForms.dbName WHERE DbStaticForms.dbName IS NULL");

            Alter.Table("TFMessages").AddColumn("FormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id"); ;
            Execute.Sql("UPDATE TFMessages SET FormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticForms.dbName = TFMessages.FormDbName");

        }
    }
}
