using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFRPPointRepository : IRPPointRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<RPPoint> RPPoints
        {
            get { return context.RPPoints; }
        }

        public void SaveRPPoint(RPPoint RPPoint)
        {
            if (RPPoint.Id == 0)
            {
                context.RPPoints.Add(RPPoint);
            }
            else
            {
                RPPoint editMe = context.RPPoints.Find(RPPoint.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = RPPoint.Name;
                    // dbEntry.Message = RPPoint.Message;
                    // dbEntry.TimeStamp = RPPoint.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteRPPoint(int id)
        {

            RPPoint dbEntry = context.RPPoints.Find(id);
            if (dbEntry != null)
            {
                context.RPPoints.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}