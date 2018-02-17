using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFContributorCustomFormRepository : IContributorCustomFormRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<ContributorCustomForm> ContributorCustomForms
        {
            get { return context.ContributorCustomForms; }
        }

        public void SaveContributorCustomForm(ContributorCustomForm ContributorCustomForm)
        {
            if (ContributorCustomForm.Id == 0)
            {
                context.ContributorCustomForms.Add(ContributorCustomForm);
            }
            else
            {
                var editMe = context.ContributorCustomForms.Find(ContributorCustomForm.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = ContributorCustomForm.Name;
                    // dbEntry.Message = ContributorCustomForm.Message;
                    // dbEntry.TimeStamp = ContributorCustomForm.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteContributorCustomForm(int id)
        {

            var dbEntry = context.ContributorCustomForms.Find(id);
            if (dbEntry != null)
            {
                context.ContributorCustomForms.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}