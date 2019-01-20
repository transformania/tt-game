using System.Linq;
using Highway.Data;
using TT.Domain.Forms.Entities;

namespace TT.Domain.Players.Queries
{
    public class IsBaseForm : DomainQuerySingle<bool>
    {

        public int formSourceId { get; set; }

        public override bool Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var formSource = ctx.AsQueryable<FormSource>()
                    .FirstOrDefault(m => m.Id == formSourceId);
               
                if (formSource == null)
                    return false;

                return formSource.FriendlyName == "Regular Guy" || formSource.FriendlyName == "Regular Girl";
            };

            return ExecuteInternal(context);
        }

    }
}

