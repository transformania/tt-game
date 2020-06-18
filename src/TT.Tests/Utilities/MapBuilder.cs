using AutoMapper;
using TT.Domain;

namespace TT.Tests.Utilities
{
    public class MapBuilder
    {
        public IMapper BuildMapper()
        {
            return new MapperConfiguration(cfg =>
            {
#pragma warning disable 618
                cfg.CreateMissingTypeMaps = true;
#pragma warning restore 618
                cfg.AddMaps(typeof(DomainRegistry).Assembly);
            }).CreateMapper();
        }
    }
}
