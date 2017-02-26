using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries
{
    public class GetItemSources : DomainQuery<ItemSourceDetail>
    {
        public override IEnumerable<ItemSourceDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<ItemSource>().Select(cr => new ItemSourceDetail
                {
                    Id = cr.Id,
                    DbName = cr.DbName,
                    FriendlyName = cr.FriendlyName,
                    IsUnique = cr.IsUnique
                });

            };

            return ExecuteInternal(context);
        }
    }
}