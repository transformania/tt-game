using System.Collections.Generic;
using System.Data.Entity;
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
                var bios = ctx.AsQueryable<ArtistBio>()
                           .Include(p => p.Owner)
                           .Where(p => p.IsLive == true)
                           .OrderBy(p => p.OtherNames)
                           .ToList();

                return bios.Select(p => p.MapToDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}
