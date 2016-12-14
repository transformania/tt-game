using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Highway.Data;
using TT.Domain.DTOs.Messages;
using TT.Domain.DTOs.Players;
using TT.Domain.Entities.Forms;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Queries.Players
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
