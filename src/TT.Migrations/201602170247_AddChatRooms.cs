using System;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201602170247)]
    public class AddChatRooms : Migration
    {
        public override void Up()
        {
            Create.Table("ChatRooms")
                .WithColumn("Id").AsInt32().Identity().NotNullable().PrimaryKey()
                .WithColumn("Creator_Id").AsString(128).NotNullable().ForeignKey("AspNetUsers", "Id").Indexed()
                .WithColumn("Name").AsString().NotNullable().Indexed()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();

            Insert.IntoTable("AspNetUsers")
                .Row(new
            {
                Id = "1",
                AccessFailedCount = 0,
                LockoutEnabled = true,
                PasswordHash = "ANaeYOYgZsL5O+rOoUAvvVBR4x+zjJ6GTEjRX5ZLRsc+2gvELRoEn4M0BfZ42ha2+g==",
                SecurityStamp = "28069774-ce7a-4add-bead-db9bb04c7976",
                UserName = "system",
                CreateDate = DateTime.Now,
            });

            Insert.IntoTable("ChatRooms").Row(new
            {
                Creator_Id = "1",
                Name = "Global",
                CreatedAt = DateTime.Now,
            });
        }

        public override void Down()
        {
            
        }
    }
}