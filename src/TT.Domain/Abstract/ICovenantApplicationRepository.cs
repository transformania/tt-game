using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface ICovenantApplicationRepository
    {

        IQueryable<CovenantApplication> CovenantApplications { get; }

        void SaveCovenantApplication(CovenantApplication CovenantApplication);

        void DeleteCovenantApplication(int CovenantApplicationId);

    }
}