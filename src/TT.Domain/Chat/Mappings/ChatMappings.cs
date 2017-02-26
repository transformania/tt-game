using System.Data.Entity;
using Highway.Data;
using TT.Domain.Chat.Entities;

namespace TT.Domain.Chat.Mappings
{
    public class ChatMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatRoom>()
                .ToTable("ChatRooms")
                .HasKey(cr => cr.Id)
                .HasRequired(cr => cr.Creator).WithMany();
        }
    }
}