using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFSelfRestoreEnergies : ISelfRestoreEnergies
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Models.SelfRestoreEnergies> SelfRestoreEnergies
        {
            get
            { 
                return context.SelfRestoreEnergies;
            }
        }

        public void DeleteSelfRestoreEnergies(int id)
        {
            var dbEntry = context.SelfRestoreEnergies.Find(id);
            if (dbEntry != null)
            {
                context.SelfRestoreEnergies.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}