﻿using System.Data.Entity;
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

        public MessageMappings()
        {
            CreateMap<Message, MessageDetail>().ForMember(dst => dst.MessageId, opt => opt.MapFrom(src => src.Id));
        }
    }
}