using System.Data.Entity;
using Highway.Data;
using TT.Domain.Effects.Entities;

namespace TT.Domain.Effects.Mappings
{
    public class EffectSourceMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EffectSource>()
                .ToTable("DbStaticEffects")
                .HasKey(e => e.Id);
        }
    }
}
