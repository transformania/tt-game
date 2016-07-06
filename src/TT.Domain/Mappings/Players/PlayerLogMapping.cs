using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Players;
using TT.Domain.Entities.Players;

namespace TT.Domain.Mappings.Players
{
    public class PlayerLogMapping : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerLog>()
                .ToTable("PlayerLogs")
                .HasKey(cr => cr.Id);


            modelBuilder.Entity<PlayerLog>()
                .HasRequired(p => p.Owner)
                .WithMany(p => p.PlayerLogs).Map(p => p.MapKey("PlayerId"));

        }

        protected override void Configure()
        {
            CreateMap<PlayerLog, PlayerLogDetail>();
        }
    }
}
