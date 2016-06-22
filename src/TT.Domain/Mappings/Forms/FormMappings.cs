using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Forms;

namespace TT.Domain.Mappings.Item
{
    public class FormMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Forms.FormSource>()
                .ToTable("DbStaticForms")
                .HasKey(cr => cr.Id);

           
        }

        protected override void Configure()
        {
            CreateMap<Entities.Forms.FormSource, FormSourceDetail>();
        }
    }
}