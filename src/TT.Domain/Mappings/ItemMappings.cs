using System.Data.Entity;
using Highway.Data;
using TT.Domain.Entities.Items;

namespace TT.Domain.Mappings
{
    public class ItemMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemSource>()
                .ToTable("DbStaticItems")
                .HasKey(cr => cr.Id);
        }
    }
}