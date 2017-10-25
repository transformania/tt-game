using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Covenants.Entities;
using TT.Domain.Identity.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Covenants.Mappings
{
    public class CovenantMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Covenant>()
                .ToTable("Covenants")
                .HasKey(e => e.Id);

            modelBuilder.Entity<User>()
                .HasOptional(p => p.CovenantFounded)
                .WithRequired(d => d.Founder)
                .Map(m => m.MapKey("FounderMembershipId"));

            modelBuilder.Entity<Player>()
                .HasOptional(p => p.CovenantLed)
                .WithRequired(d => d.Leader)
                .Map(m => m.MapKey("LeaderId"));

        }

        public CovenantMappings()
        {

        }
    }
}
