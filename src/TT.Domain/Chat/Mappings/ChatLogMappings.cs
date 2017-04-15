using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Chat.DTOs;
using TT.Domain.Chat.Entities;

namespace TT.Domain.Chat.Mappings
{
    public class ChatLogMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatLog>()
                .ToTable("ChatLogs")
                .HasKey(c => c.Id);

        }

        public ChatLogMappings()
        {
            CreateMap<ChatLog, ChatLogDetail>();
        }
    }
}