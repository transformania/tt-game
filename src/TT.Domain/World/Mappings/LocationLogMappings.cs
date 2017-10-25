using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.World.DTOs;

namespace TT.Domain.World.Mappings
{
    public class LocationLogMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocationLog>()
               .ToTable("LocationLogs")
               .HasKey(l => l.Id);

        }

        public LocationLogMappings()
        {
            CreateMap<LocationLog, LocationLogDetail>();
        }
    }
}
