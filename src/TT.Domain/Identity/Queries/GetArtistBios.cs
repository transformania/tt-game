using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class GetArtistBios : DomainQuery<ArtistBioDetail>
    {

        public override IEnumerable<ArtistBioDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<ArtistBio>()
                           .Where(p => p.IsLive == true)
                           .OrderBy(p => p.OtherNames)
                           .ProjectToQueryable<ArtistBioDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
