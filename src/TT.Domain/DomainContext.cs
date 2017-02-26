using System.Data.Entity;
using Highway.Data;
using System.Linq;
using TT.Domain.Interfaces;

namespace TT.Domain
{
    public class DomainContext : DataContext
    {
        public DomainContext() : base("StatsWebConnection", new EntityMappings())
        {
            Database.SetInitializer(new NullDatabaseInitializer<DomainContext>());
        }

        public override int Commit()
        {
            foreach (var entry in ChangeTracker.Entries<IRemovable>().Where(entry => entry.Entity.Removed))
            {
                entry.State = System.Data.Entity.EntityState.Deleted;
            }

            return base.Commit();
        }
    }
}