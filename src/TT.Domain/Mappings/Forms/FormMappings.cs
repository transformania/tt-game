using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Forms;
using TT.Domain.DTOs.Item;
using TT.Domain.Entities.Forms;

namespace TT.Domain.Mappings.Item
{
    public class FormMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Items.Item>()
                .ToTable("Items")
                .HasKey(cr => cr.Id);

           
        }

        protected override void Configure()
        {
            CreateMap<Entities.Forms.FormSource, FormSourceDetail>();
        }
    }
}