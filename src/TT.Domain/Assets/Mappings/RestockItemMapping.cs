using System.Data.Entity;
using Highway.Data;
using TT.Domain.Assets.Entities;

namespace TT.Domain.Assets.Mappings
{
    public class RestockItemMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RestockItem>()
                .ToTable("RestockItems")
                .HasKey(cr => cr.Id)
                .HasRequired(cr => cr.BaseItem).WithMany();

        }
    }
}