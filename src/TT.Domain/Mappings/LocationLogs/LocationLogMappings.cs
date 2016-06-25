using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.LocationLog;
using TT.Domain.Entities.LocationLogs;

namespace TT.Domain.Mappings.LocationLogs
{
    public class LocationLogMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocationLog>()
               .ToTable("LocationLogs")
               .HasKey(l => l.Id);

        }

        protected override void Configure()
        {
            CreateMap<LocationLog, LocationLogDetail>();
        }
    }
}
