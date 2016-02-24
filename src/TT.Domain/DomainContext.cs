using Highway.Data;
using TT.Domain.Mappings;

namespace TT.Domain
{
    public class DomainContext : DataContext
    {
        public DomainContext() : base("StatsWebConnection", new EntityMappings()) { }
    }
}