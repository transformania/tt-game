using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Forms.DTOs;
using TT.Domain.Forms.Entities;

namespace TT.Domain.Players.Queries
{
    public class GetBaseForms : DomainQuery<BaseFormDetail>
    {
        public override IEnumerable<BaseFormDetail> Execute(IDataContext context)
        {
            ContextQuery =
                ctx =>
                    ctx.AsQueryable<FormSource>()
                        .Where(m => m.FriendlyName == "Regular Guy" || m.FriendlyName == "Regular Girl")
                        .ProjectToQueryable<BaseFormDetail>().OrderByDescending(f => f.FriendlyName);
            return ExecuteInternal(context);
        }
    }
}
