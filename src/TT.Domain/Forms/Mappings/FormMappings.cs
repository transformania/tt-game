using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Combat.DTOs;
using TT.Domain.Entities.TFEnergies;
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

            // TODO:  TFMessage should require having a FormSource in the future, but for now
            // because forms are produced by an old publish that can't set the FK immediately,
            // TF messages need to have an optional mapping to FormSource.
            modelBuilder.Entity<FormSource>()
                .HasOptional(p => p.TfMessage)
                .WithOptionalPrincipal(d => d.FormSource)
                .Map(m => m.MapKey("FormSourceId"));

        }

        protected override void Configure()
        {
            CreateMap<FormSource, FormSourceDetail>();
            CreateMap<TFEnergy, TFEnergyDetail>();
            CreateMap<TFMessage, TFMessageDetail>();
        }
    }
}