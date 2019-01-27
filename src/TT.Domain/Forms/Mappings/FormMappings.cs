using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Forms.DTOs;
using TT.Domain.Forms.Entities;

namespace TT.Domain.Forms.Mappings
{
    public class FormMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FormSource>()
                .ToTable("DbStaticForms")
                .HasKey(cr => cr.Id);

            modelBuilder.Entity<TFMessage>()
                .ToTable("TFMessages")
                .HasKey(cr => cr.Id);

            modelBuilder.Entity<FormSource>()
                .HasOptional(cr => cr.ItemSource)
                .WithMany().Map(m => m.MapKey("ItemSourceId"));

            modelBuilder.Entity<FormSource>()
                .HasOptional(p => p.TfMessage)
                .WithOptionalPrincipal(d => d.FormSource)
                .Map(m => m.MapKey("FormSourceId"));

            modelBuilder.Entity<FormSource>()
                .HasOptional(i => i.AltSexFormSource)
                .WithOptionalPrincipal()
               .Map(m => m.MapKey("AltSexFormSourceId"));

        }

        public FormMappings()
        {
            CreateMap<FormSource, FormSourceDetail>();
            CreateMap<FormSource, BaseFormDetail>();
            CreateMap<TFMessage, TFMessageDetail>();
        }
    }
}