using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Mappings
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

        public PlayerLogMapping()
        {
            CreateMap<PlayerLog, PlayerLogDetail>();
        }
    }
}
