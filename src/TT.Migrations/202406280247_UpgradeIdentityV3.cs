using FluentMigrator;

namespace TT.Migrations;

[Migration(202406280247)]
public class UpgradeIdentityV3 : ForwardOnlyMigration {
    public override void Up()
    {
        // Full V2 to V3 changes https://stackoverflow.com/a/65003440
        // Because of the need to run both TT.Web and TT.Server simultaneously, we can't just rename columns.
        // Once TT.Web has been retired. The old fields should be dropped.

        Alter.Table("AspNetUsers").AddColumn("NormalizedUserName").AsString(256).Nullable();
        Alter.Table("AspNetUsers").AddColumn("NormalizedEmail").AsString(256).Nullable();
        Alter.Table("AspNetUsers").AddColumn("LockoutEnd").AsDateTime().Nullable();
        Alter.Table("AspNetUsers").AddColumn("ConcurrencyStamp").AsString().Nullable();

        Alter.Table("AspNetRoles").AddColumn("NormalizedName").AsString(256).Nullable();
        Alter.Table("AspNetRoles").AddColumn("ConcurrencyStamp").AsString().Nullable();

        Alter.Table("AspNetUserLogins").AddColumn("ProviderDisplayName").AsString().Nullable();

        Create.Table("AspNetRoleClaims")
            .WithColumn("Id").AsInt32().Identity().PrimaryKey("PK_AspNetRoleClaims")
            .WithColumn("RoleId").AsString(128).NotNullable()
            .WithColumn("ClaimType").AsString().Nullable()
            .WithColumn("ClaimValue").AsString().Nullable();

        Create.ForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId")
            .FromTable("AspNetRoleClaims")
            .ForeignColumn("RoleId")
            .ToTable("AspNetRoles")
            .PrimaryColumn("Id");

        Create.Table("AspNetUserTokens")
            .WithColumn("UserId").AsString(128).NotNullable()
            .WithColumn("LoginProvider").AsString(450).NotNullable()
            .WithColumn("Name").AsString(450).NotNullable()
            .WithColumn("Value").AsString().Nullable();

        Create.PrimaryKey("PK_AspNetUserTokens")
            .OnTable("AspNetUserTokens")
            .Columns("UserId", "LoginProvider", "Name");

        Create.ForeignKey("FK_AspNetUserTokens_AspNetUsers_UserId")
            .FromTable("AspNetUserTokens")
            .ForeignColumn("UserId")
            .ToTable("AspNetUsers")
            .PrimaryColumn("Id");

        Execute.Sql("UPDATE AspNetUsers SET NormalizedUserName = UPPER(UserName) WHERE NormalizedUserName IS NULL");
        Execute.Sql("UPDATE AspNetUsers SET NormalizedEmail = UPPER(Email) WHERE NormalizedEmail IS NULL");
        Execute.Sql("UPDATE AspNetUsers SET LockoutEnd = LockoutEndDateUtc WHERE LockoutEndDateUtc IS NOT NULL");
        Execute.Sql("UPDATE AspNetRoles SET NormalizedName = UPPER(Name) WHERE NormalizedName IS NULL");
    }
}
