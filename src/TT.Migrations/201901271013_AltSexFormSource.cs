using System;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201901271013)]
    public class AltSexFormSource : ForwardOnlyMigration
    {
        public override void Up()
        {

            Alter.Table("MindControls").AddColumn("FormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");
            Execute.Sql("UPDATE MindControls SET FormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticForms.dbName = MindControls.Type");

            Alter.Table("DbStaticForms").AddColumn("AltSexFormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");

            // male to female
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 7 FROM DbStaticForms WHERE DbStaticForms.Id = 2");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 8 FROM DbStaticForms WHERE DbStaticForms.Id = 3");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 9 FROM DbStaticForms WHERE DbStaticForms.Id = 4");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 10 FROM DbStaticForms WHERE DbStaticForms.Id = 5");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 11 FROM DbStaticForms WHERE DbStaticForms.Id = 6");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 745 FROM DbStaticForms WHERE DbStaticForms.Id = 742");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 746 FROM DbStaticForms WHERE DbStaticForms.Id = 743");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 747 FROM DbStaticForms WHERE DbStaticForms.Id = 744");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 754 FROM DbStaticForms WHERE DbStaticForms.Id = 753");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 764 FROM DbStaticForms WHERE DbStaticForms.Id = 763");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 766 FROM DbStaticForms WHERE DbStaticForms.Id = 765");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 768 FROM DbStaticForms WHERE DbStaticForms.Id = 767");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 770 FROM DbStaticForms WHERE DbStaticForms.Id = 769");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 779 FROM DbStaticForms WHERE DbStaticForms.Id = 778");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 781 FROM DbStaticForms WHERE DbStaticForms.Id = 780");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 784 FROM DbStaticForms WHERE DbStaticForms.Id = 783");

            // female to male
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 2 FROM DbStaticForms WHERE DbStaticForms.Id = 7");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 3 FROM DbStaticForms WHERE DbStaticForms.Id = 8");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 4 FROM DbStaticForms WHERE DbStaticForms.Id = 9");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 5 FROM DbStaticForms WHERE DbStaticForms.Id = 10");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 6 FROM DbStaticForms WHERE DbStaticForms.Id = 11");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 742 FROM DbStaticForms WHERE DbStaticForms.Id = 745");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 743 FROM DbStaticForms WHERE DbStaticForms.Id = 746");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 744 FROM DbStaticForms WHERE DbStaticForms.Id = 747");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 753 FROM DbStaticForms WHERE DbStaticForms.Id = 754");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 763 FROM DbStaticForms WHERE DbStaticForms.Id = 764");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 765 FROM DbStaticForms WHERE DbStaticForms.Id = 766");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 767 FROM DbStaticForms WHERE DbStaticForms.Id = 768");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 769 FROM DbStaticForms WHERE DbStaticForms.Id = 770");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 778 FROM DbStaticForms WHERE DbStaticForms.Id = 779");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 780 FROM DbStaticForms WHERE DbStaticForms.Id = 781");
            Execute.Sql("UPDATE DbStaticForms SET AltSexFormSourceId = 783 FROM DbStaticForms WHERE DbStaticForms.Id = 784");

        }
    }
}
