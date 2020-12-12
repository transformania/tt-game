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
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Rusty Steamstein', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Rusty Bartender', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Rusty Automaton', GETUTCDATE());");
            //Jewdewfae
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Jewdewfae Pervfae', GETUTCDATE());");
            //Lindella
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lindella Vendor', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lindella Soul', GETUTCDATE());");
            //Snek
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Skaldrlyr Forbidden', GETUTCDATE());");
            //Woof
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wüffie Soul', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wüffie Pet', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wüffie Vendor', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wuffie Soul', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wuffie Pet', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Wuffie Vendor', GETUTCDATE());");
            //Succubutt
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Karin Kezesul-Adriz', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Karin Soulbinder', GETUTCDATE());");

            //Bosses
            //Bimboss
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lady Lovebringer', GETUTCDATE());");
            //Not-Circe
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Donna Milton', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Aunt Donna', GETUTCDATE());");
            //Faeboss
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Narcissa Exile', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Narcissa Exiled', GETUTCDATE());");
            //Vroom Boss
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Road Queen', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Harley Punksplitter', GETUTCDATE());");
            //Nerd Sister
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Headmistress Brisby', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Adrianna Brisby', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Headmistress Adrianna', GETUTCDATE());");
            //Bimbo Sister
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Beautician Brisby', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Beautician Candice', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Candice Brisby', GETUTCDATE());");
            //Brother Thief
            Execute.Sql("INSERT INTO ReservedNames( FullName, TimeStamp) VALUES('Brother Seekshadow', GETUTCDATE()); ");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lujako Seekshadow', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Brother Lujako', GETUTCDATE());");
            //Sister Thied
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Sister Seekshadow', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lujienne Seekshadow', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Sister Lujienne', GETUTCDATE());");
            //Krampus
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lord Valentine', GETUTCDATE());");
            Execute.Sql("INSERT INTO ReservedNames (FullName,TimeStamp) VALUES ('Lady Krampus', GETUTCDATE());");

        }
    }
}