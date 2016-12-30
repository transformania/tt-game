using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Identity;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Queries.Identity
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
