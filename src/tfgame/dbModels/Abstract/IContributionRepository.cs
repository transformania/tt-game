using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IContributionRepository
    {

        IQueryable<Contribution> Contributions { get; }

        void SaveContribution(Contribution Contribution);

        void DeleteContribution(int ContributionId);

    }
}