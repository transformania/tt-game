using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IRPPointRepository
    {

        IQueryable<RPPoint> RPPoints { get; }

        void SaveRPPoint(RPPoint RPPoint);

        void DeleteRPPoint(int RPPointId);

    }
}