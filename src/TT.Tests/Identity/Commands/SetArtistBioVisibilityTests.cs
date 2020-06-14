using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Commands
{
    [TestFixture]
    public class SetArtistBioVisibilityTests : TestBase
    {
        [Test]
        public void should_set_bio_visibility()
        {

            var bio = new ArtistBioBuilder().With(i => i.Id, 3)
                 .With(cr => cr.Owner, new UserBuilder().With(u => u.Id, "bob").BuildAndSave())
                 .With(b => b.OtherNames, "Artist Bob")
                 .With(b => b.IsLive, false)
                 .BuildAndSave();

            var output = DomainRegistry.Repository.Execute(new SetArtistBioVisibility { UserId = bio.Owner.Id, IsVisible = true });

            var editedBio = DataContext.AsQueryable<ArtistBio>().First(b => b.Owner.Id == bio.Owner.Id);
            Assert.That(editedBio.IsLive, Is.True);
            Assert.That(output, Is.EqualTo("You set your artist bio visibility to True."));
        }

        [Test]
        public void should_throw_exception_if_bio_not_found()
        {
            var cmd = new SetArtistBioVisibility { UserId = "fake", IsVisible = true };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Artist bio for user 'fake' could not be found"));
        }

        [Test]
        public void should_throw_exception_if_userId_not_provided()
        {
            var cmd = new SetArtistBioVisibility { IsVisible = true };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("userId is required"));
        }

    }
}