using System.Data.Entity;
using Highway.Data;
using TT.Domain.Assets.Entities;

namespace TT.Domain.Assets.Mappings
{
    public class AssetsMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tome>()
                .ToTable("Tomes")
                .HasKey(cr => cr.Id)
                .HasRequired(cr => cr.BaseItem).WithMany();
        }
    }
}