using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Messages.DTOs;
using TT.Domain.Messages.Entities;

namespace TT.Domain.Messages.Mappings
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