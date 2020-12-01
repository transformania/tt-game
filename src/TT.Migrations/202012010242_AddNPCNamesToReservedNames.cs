using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202012010242)]
    public class AddNPCNamesToReservedNames : ForwardOnlyMigration
    {
        public override void Up()
        {

            //Reserving all names for NPCs under my membership ID, because.

            //NPCs and Vendors
            //Rusty
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Rusty Steamstein', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Rusty Bartender', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Rusty Automaton', GETDATE());");
            //Jewdewfae
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Jewdewfae Pervfae', GETDATE());");
            //Lindella
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Lindella Vendor', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Lindella Soul', GETDATE());");
            //Snek
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Skaldrlyr Forbidden', GETDATE());");
            //Woof
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Wüffie Soul', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Wüffie Pet', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Wüffie Vendor', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Wuffie Soul', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Wuffie Pet', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Wuffie Vendor', GETDATE());");
            //Succubutt
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Karin Kezesul-Adriz', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Karin Soulbinder', GETDATE());");

            //Bosses
            //Bimboss
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Lady Lovebringer', GETDATE());");
            //Not-Circe
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Donna Milton', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Aunt Donna', GETDATE());");
            //Faeboss
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Narcissa Exile', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Narcissa Exiled', GETDATE());");
            //Vroom Boss
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Road Queen', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Harley Punksplitter', GETDATE());");
            //Nerd Sister
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Headmistress Brisby', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Adrianna Brisby', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Headmistress Adrianna', GETDATE());");
            //Bimbo Sister
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Beautician Brisby', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Beautician Candice', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Candice Brisby', GETDATE());");
            //Brother Thief
            Execute.Sql("INSERT INTO ReservedNames(MembershipId, FullName, TimeStamp) VALUES('7570', 'Brother Seekshadow', GETDATE()); ");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Lujako Seekshadow', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Brother Lujako', GETDATE());");
            //Sister Thied
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Sister Seekshadow', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Lujienne Seekshadow', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Sister Lujienne', GETDATE());");
            //Krampus
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Lord Valentine', GETDATE());");
            Execute.Sql("INSERT INTO ReservedNames (MembershipId,FullName,TimeStamp) VALUES ('7570', 'Lady Krampus', GETDATE());");

        }
    }
}
