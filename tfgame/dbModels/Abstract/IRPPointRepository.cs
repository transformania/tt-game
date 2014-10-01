using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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