using System;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201901251339)]
    public class QuestSetFormMigration : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql("UPDATE QuestStatePreactions SET ActionValue = DbStaticForms.Id FROM DbStaticForms WHERE QuestStatePreactions.ActionValue = DbStaticForms.dbName");
        }
    }
}
