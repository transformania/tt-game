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
                        .OrderByDescending(f => f.FriendlyName)
                        .Select(f => new BaseFormDetail
                        {
                            Id = f.Id,
                            FriendlyName = f.FriendlyName,
                            PortraitUrl = f.PortraitUrl
                        });
            return ExecuteInternal(context);
        }
    }
}
