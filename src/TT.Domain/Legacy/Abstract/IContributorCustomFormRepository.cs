using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IContributorCustomFormRepository
    {

        IQueryable<ContributorCustomForm> ContributorCustomForms { get; }

        void SaveContributorCustomForm(ContributorCustomForm ContributorCustomForm);

        void DeleteContributorCustomForm(int ContributorCustomFormId);

    }
}