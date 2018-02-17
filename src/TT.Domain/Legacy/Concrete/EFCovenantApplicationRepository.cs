using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFCovenantApplicationRepository : ICovenantApplicationRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<CovenantApplication> CovenantApplications
        {
            get { return context.CovenantApplications; }
        }

        public void SaveCovenantApplication(CovenantApplication CovenantApplication)
        {
            if (CovenantApplication.Id == 0)
            {
                context.CovenantApplications.Add(CovenantApplication);
            }
            else
            {
                var editMe = context.CovenantApplications.Find(CovenantApplication.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = CovenantApplication.Name;
                    // dbEntry.Message = CovenantApplication.Message;
                    // dbEntry.TimeStamp = CovenantApplication.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteCovenantApplication(int id)
        {

            var dbEntry = context.CovenantApplications.Find(id);
            if (dbEntry != null)
            {
                context.CovenantApplications.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}