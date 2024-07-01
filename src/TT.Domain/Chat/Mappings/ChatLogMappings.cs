using System.Data.Entity;
using Highway.Data;
using TT.Domain.Chat.Entities;

namespace TT.Domain.Chat.Mappings
{
    public class ChatLogMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatLog>()
                .ToTable("ChatLogs")
                .HasKey(c => c.Id);

        }
    }
}