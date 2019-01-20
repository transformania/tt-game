using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Entities.TFEnergies;

namespace TT.Domain.TFEnergies.Mappings
{
    public class SelfRestoreMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SelfRestoreEnergy>()
                .ToTable("SelfRestoreEnergies")
                .HasKey(cr => cr.Id);

            modelBuilder.Entity<SelfRestoreEnergy>()
                .HasRequired(i => i.Owner)
                .WithOptional(p => p.SelfRestoreEnergy).Map(m => m.MapKey("OwnerId"));

        }

    }
}