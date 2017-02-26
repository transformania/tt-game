using System;
using TT.Domain.Identity.Entities;

namespace TT.Tests.Builders.Identity
{
    public class ArtistBioBuilder : Builder<ArtistBio, int>
    {
        public ArtistBioBuilder()
        {
            Instance = Create();
            With(u => u.Id, 7);
        }
    }
}
