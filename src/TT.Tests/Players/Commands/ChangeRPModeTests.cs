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
    public class ChangeRPModeTests : TestBase
    {
        [Test]
        public void should_put_player_into_RP_mode_when_not()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.InRP, false)
                .BuildAndSave();

            Assert.That(
                () => DomainRegistry.Repository.Execute(new ChangeRPMode
                    {MembershipId = player.User.Id, InRPMode = true}), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").InRP, Is.True);
        }

        [Test]
        public void should_put_player_into_not_RP_mode_when_in()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.InRP, true)
                .BuildAndSave();

            Assert.That(
                () => DomainRegistry.Repository.Execute(new ChangeRPMode
                    {MembershipId = player.User.Id, InRPMode = false}), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").InRP, Is.False);
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeRPMode {MembershipId = "fake", InRPMode = true};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Player with MembershipID 'fake' could not be found"));
        }

        [Test]
        public void Should_throw_exception_if_membership_null()
        {
            var cmd = new ChangeRPMode { MembershipId = null, InRPMode = true };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("MembershipID is required!"));
        }
    }
}
