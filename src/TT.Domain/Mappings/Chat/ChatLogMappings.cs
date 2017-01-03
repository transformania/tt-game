using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Entities.Chat;

namespace TT.Domain.Mappings.Item
{
    public class ChatLogMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatLog>()
                .ToTable("ChatLogs")
                .HasKey(c => c.Id);

        }

        protected override void Configure()
        {

        }
    }
}