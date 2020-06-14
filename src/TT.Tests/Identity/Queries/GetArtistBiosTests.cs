using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
{
    [TestFixture]
    public class GetArtistBiosTests : TestBase
    {
        [Test]
        public void get_all_live_artist_bios()
        {

            var live_bio = new ArtistBioBuilder().With(i => i.Id, 3)
                .With(cr => cr.Owner, new UserBuilder().With(u => u.Id, "bob").BuildAndSave())
                .With(b => b.OtherNames, "Artist Bob")
                .With(b => b.IsLive, true)
                .BuildAndSave();

            new ArtistBioBuilder().With(i => i.Id, 4)
                .With(cr => cr.Owner, new UserBuilder().With(u => u.Id, "tom").BuildAndSave())
                .With(b => b.OtherNames, "Artist Tom")
                .With(b => b.IsLive, false)
                .BuildAndSave();

            var bios = DomainRegistry.Repository.Find(new GetArtistBios()).ToArray();

            Assert.That(bios, Has.Exactly(1).Items);
            Assert.That(bios[0].Id, Is.EqualTo(live_bio.Id));
        }

    }
}