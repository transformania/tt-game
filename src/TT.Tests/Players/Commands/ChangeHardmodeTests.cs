using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Players.Commands
{
    [TestFixture()]
    public class ChangeHardmodeTests : TestBase
    {
        [Test]
        public void should_put_player_into_hardmode_when_not()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.InHardmode, false)
                .BuildAndSave();

            Assert.That(
                () => DomainRegistry.Repository.Execute(new ChangeHardmode
                    {MembershipId = player.User.Id, InHardmode = true}), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").InHardmode, Is.True);
        }

        [Test]
        public void should_do_nothing_when_in()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.InHardmode, true)
                .BuildAndSave();

            Assert.That(
                () => DomainRegistry.Repository.Execute(new ChangeHardmode
                    {MembershipId = player.User.Id, InHardmode = true}), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").InHardmode, Is.True);
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeHardmode {MembershipId = "fake", InHardmode = true};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Player with MembershipID 'fake' could not be found"));
        }

        [Test]
        public void Should_throw_exception_if_membership_null()
        {
            var cmd = new ChangeHardmode { MembershipId = null, InHardmode = true };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("MembershipID is required!"));
        }
    }
}
