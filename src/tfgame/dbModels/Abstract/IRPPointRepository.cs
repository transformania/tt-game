using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IRPPointRepository
    {

        IQueryable<RPPoint> RPPoints { get; }

        void SaveRPPoint(RPPoint RPPoint);

        void DeleteRPPoint(int RPPointId);

    }
}