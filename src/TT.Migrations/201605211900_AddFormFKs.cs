﻿using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201605211900)]
    public class AddFormFKs : ForwardOnlyMigration
    {
        
        public override void Up()
        {
            Alter.Table("Players").AddColumn("FormSourceId").AsInt32().Nullable();
            Execute.Sql("UPDATE Players SET FormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticForms.dbName = Players.Form");
            Alter.Table("Players").AlterColumn("FormSourceId").AsInt32().NotNullable().ForeignKey("DbStaticForms", "Id");
        }

    }
}
