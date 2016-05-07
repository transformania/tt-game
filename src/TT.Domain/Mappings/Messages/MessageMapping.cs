using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Messages;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Mappings.Messages
{
    public class MessageMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .ToTable("Messages")
                .HasKey(cr => cr.Id)
                .HasRequired(cr => cr.Sender)
                .WithMany().Map(m => m.MapKey("SenderId"));

            modelBuilder.Entity<Message>()
                .HasRequired(cr => cr.Receiver)
                .WithMany().Map(m => m.MapKey("ReceiverId"));
        }

        protected override void Configure()
        {
            CreateMap<Message, MessageDetail>();
        }
    }
}