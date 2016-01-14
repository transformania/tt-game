using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface ICovenantApplicationRepository
    {

        IQueryable<CovenantApplication> CovenantApplications { get; }

        void SaveCovenantApplication(CovenantApplication CovenantApplication);

        void DeleteCovenantApplication(int CovenantApplicationId);

    }
}