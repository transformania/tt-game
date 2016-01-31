using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IContributionRepository
    {

        IQueryable<Contribution> Contributions { get; }

        void SaveContribution(Contribution Contribution);

        void DeleteContribution(int ContributionId);

    }
}