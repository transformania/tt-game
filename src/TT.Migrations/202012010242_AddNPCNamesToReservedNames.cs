using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202012010242)]
    public class AddNPCNamesToReservedNames : ForwardOnlyMigration
    {
        public override void Up()
        {

            //NPCs and Vendors
            //Rusty
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Rusty Steamstein', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Rusty Bartender', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Rusty Automaton', GETDATE());");
            //Jewdewfae
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Jewdewfae Pervfae', GETDATE());");
            //Lindella
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lindella Vendor', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lindella Soul', GETDATE());");
            //Snek
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Skaldrlyr Forbidden', GETDATE());");
            //Woof
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wüffie Soul', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wüffie Pet', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wüffie Vendor', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wuffie Soul', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wuffie Pet', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wuffie Vendor', GETDATE());");
            //Succubutt
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Karin Kezesul-Adriz', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Karin Soulbinder', GETDATE());");

            //Bosses
            //Bimboss
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lady Lovebringer', GETDATE());");
            //Not-Circe
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Donna Milton', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Aunt Donna', GETDATE());");
            //Faeboss
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Narcissa Exile', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Narcissa Exiled', GETDATE());");
            //Vroom Boss
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Road Queen', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Harley Punksplitter', GETDATE());");
            //Nerd Sister
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Headmistress Brisby', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Adrianna Brisby', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Headmistress Adrianna', GETDATE());");
            //Bimbo Sister
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Beautician Brisby', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Beautician Candice', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Candice Brisby', GETDATE());");
            //Brother Thief
            Execute.Sql("INSERT INTO ReservedNames( FullName, TimeStamp) VALUES('Brother Seekshadow', GETDATE()); ");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lujako Seekshadow', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Brother Lujako', GETDATE());");
            //Sister Thied
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Sister Seekshadow', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lujienne Seekshadow', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Sister Lujienne', GETDATE());");
            //Krampus
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lord Valentine', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lady Krampus', GETDATE());");

        }
    }
}