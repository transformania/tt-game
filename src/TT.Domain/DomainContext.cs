using Highway.Data;
using System.Linq;
using TT.Domain.Interfaces;
using TT.Domain.Mappings;

namespace TT.Domain
{
    public class DomainContext : DataContext
    {
        public DomainContext() : base("StatsWebConnection", new EntityMappings()) { }

        public override int Commit()
        {
            foreach (var entry in ChangeTracker.Entries<IDeletable>().Where(entry => entry.Entity.Deleted))
            {
                entry.State = System.Data.Entity.EntityState.Deleted;
            }

            return base.Commit();
        }
    }
}